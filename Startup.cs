using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductOwnerSimGame.Authentication;
using ProductOwnerSimGame.DataAccess.Implementations;
using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Hubs;
using ProductOwnerSimGame.IdentityStores;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Models.GameVariant;
using ProductOwnerSimGame.Models.Permissions;
using ProductOwnerSimGame.Models.Roles;
using ProductOwnerSimGame.Models.Users;
using ProductOwnerSimGame.Permissions;
using ProductOwnerSimGame.Simulation;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductOwnerSimGame
{
    public class Startup
    {
        private const string _SignalRRoute = "/gameviewupdates";


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddIdentity<User, Role>()
                .AddDefaultTokenProviders();

            services.AddTransient<IUserStore<User>, CustomUserStore>();
            services.AddTransient<IUserEmailStore<User>, CustomUserStore>();
            services.AddTransient<IRoleStore<Role>, CustomRoleStore>();

            services.AddAuthorization(options =>
            {
                foreach (var permission in IntegratedPermissions.All())
                {
                    options.AddPolicy(permission.Name,
                        policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

            var signingKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(Configuration["Authentication:JwtBearer:SecurityKey"]));

            var jwtTokenConfiguration = new JwtTokenConfiguration
            {
                Issuer = Configuration["Authentication:JwtBearer:Issuer"],
                Audience = Configuration["Authentication:JwtBearer:Audience"],
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(60),
            };

            services.Configure<JwtTokenConfiguration>(config =>
            {
                config.Audience = jwtTokenConfiguration.Audience;
                config.EndDate = jwtTokenConfiguration.EndDate;
                config.Issuer = jwtTokenConfiguration.Issuer;
                config.StartDate = jwtTokenConfiguration.StartDate;
                config.SigningCredentials = jwtTokenConfiguration.SigningCredentials;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtTokenConfiguration.Issuer,
                    ValidAudience = jwtTokenConfiguration.Audience,
                    IssuerSigningKey = signingKey
                };

                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments(_SignalRRoute)))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddCors();

            services.AddScoped<IAuthorizationHandler, PermissionHandler>();


            // Add AddRazorPages if the app uses Razor Pages.
            services.AddRazorPages().AddNewtonsoftJson();

            //Adds signalr
            services.AddSignalR();

            services.AddAutoMapper(typeof(Startup));

            // Generate swagger file
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductOwnerSimGame API", Version = "v1" });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();

            });
            services.AddSwaggerGenNewtonsoftSupport();

            // Data access
            //var cosmosDbConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            var cosmosDbConnectionString = Configuration["Database:ConnectionString"];

            services.AddSingleton<IGameDataAccess>(srv => new GameDataAccessCosmosDb(cosmosDbConnectionString));
            services.AddSingleton<IGameVariantDataAccess>(srv => new GameVariantDataAccessCosmosDb(cosmosDbConnectionString));
            services.AddSingleton<IOrganizationDataAccess>(srv => new OrganizationDataAccessCosmosDb(cosmosDbConnectionString));
            services.AddSingleton<IUserDataAccess>(srv => new UserDataAccessCosmosDb(cosmosDbConnectionString));

            // Logic
            services.AddTransient<IGameLogic, GameLogic>();
            services.AddTransient<IGameViewLogic, GameViewLogic>();
            services.AddTransient<IGameVariantLogic, GameVariantLogic>();
            services.AddTransient<IOrganizationLogic, OrganizationLogic>();
            services.AddTransient<IUserLogic, UserLogic>();
            services.AddTransient<IPermissionLogic, PermissionLogic>();

            // Game controller
            services.AddSingleton<IGameManager, GameManager>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMiddleware<LogHeadersMiddleware>();

            // Enable swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductOwnerSimGame API v1");
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(
                options => options
                .WithOrigins("http://localhost:8080/", "https://productownersimgame.azurewebsites.net")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<GameViewHub>(_SignalRRoute);

                // Add MapRazorPages if the app uses Razor Pages. Since Endpoint Routing includes support for many frameworks, adding Razor Pages is now opt-in.
                endpoints.MapRazorPages();
            });

            var initialSetup = true;

            if (initialSetup)
            {
                var dataAccess = app.ApplicationServices.GetService<IUserDataAccess>();

                foreach (var user in IntegratedUsers.All())
                {
                    try
                    {
                        dataAccess.CreateUserAsync(user).Wait();



                    }
                    catch (Exception ex)
                    {

                    }
                }

                var orgDataAccess = app.ApplicationServices.GetService<IOrganizationDataAccess>();


                try
                {
                    var orgIdTask = orgDataAccess.CreateOrganizationAsync("Open IIoT Machines Platform GmbH");

                    orgIdTask.Wait();

                    var orgId = orgIdTask.Result;

                    orgDataAccess.AddUsersToOrganizationAsync(orgId, IntegratedUsers.All().Select(a => a.Id.ToString()).ToList()).Wait();
                }
                catch (Exception ex)
                {

                }

                var gvDataAccess = app.ApplicationServices.GetService<IGameVariantDataAccess>();

                try
                {
                    var gv = new GameVariant()
                    {
                        DisplayName = "Entwicklung einer Rezeptverwaltungsapplikation",
                        IsPublic = true,
                        PlayerCount = 3,
                        InitialProjectValues = new Dictionary<EffectCategory, int>() 
                        {
                            {EffectCategory.Punctuality, 30 },
                            {EffectCategory.Budget, 80 },
                            {EffectCategory.Quality, 60 },
                            {EffectCategory.StakeHolderSatisfaction, 20},
                        },
                        OverallMarkDownText = File.ReadAllText(@"./variant_data/1/overallmarkdowntext.md"),
                        GamePhases = new List<GamePhase>
                        {
                            new GamePhase
                            {
                                Sequence = 1,
                                Duration = PhaseDuration.Infinite,
                                ProductOwnerMission = new Mission
                                {
                                    MarkdownText = File.ReadAllText(@"./variant_data/1/po_mission_1.md")
                                },
                                StakeholderMissions = new List<Mission>
                                {
                                    new Mission
                                    {
                                        MarkdownText = File.ReadAllText(@"./variant_data/1/sh_mission_1.md")
                                    },
                                     new Mission
                                    {
                                        MarkdownText = File.ReadAllText(@"./variant_data/1/sh_mission_2.md")
                                    },
                                },
                                Decisions = new List<Decision>
                                {
                                    new Decision
                                    {
                                        DecisionMarkdownText =  File.ReadAllText(@"./variant_data/1/ph_1_dec_1.md"),
                                        ExplanationMarkdownText =File.ReadAllText(@"./variant_data/1/ph_1_exp_1.md"),
                                        Effects = new List<Effect>
                                        {
                                            new Effect
                                            {
                                                EffectCategory = EffectCategory.StakeHolderSatisfaction,
                                                Value = +0
                                            }
                                        }
                                    },
                                    new Decision
                                    {
                                        DecisionMarkdownText =  File.ReadAllText(@"./variant_data/1/ph_1_dec_2.md"),
                                        ExplanationMarkdownText =File.ReadAllText(@"./variant_data/1/ph_1_exp_2.md"),
                                        Effects = new List<Effect>
                                        {
                                            new Effect
                                            {
                                                EffectCategory = EffectCategory.StakeHolderSatisfaction,
                                                Value = +50
                                            }
                                        }
                                    },
                                }
                            },
                            new GamePhase
                            {
                                Sequence = 2,
                                Duration = PhaseDuration.Timeout,
                                DurationInSeconds = 15,
                                ProductOwnerMission = new Mission
                                {
                                    MarkdownText = "PO Mission 2"
                                },
                                StakeholderMissions = new List<Mission>
                                {
                                    new Mission
                                    {
                                        MarkdownText = File.ReadAllText(@"./variant_data/1/sh_mission_3.md")
                                    },
                                     new Mission
                                    {
                                        MarkdownText = File.ReadAllText(@"./variant_data/1/sh_mission_4.md")
                                    },
                                },
                                Decisions = new List<Decision>
                                {
                                    new Decision
                                    {
                                        DecisionMarkdownText =  File.ReadAllText(@"./variant_data/1/ph_2_dec_1.md"),
                                        ExplanationMarkdownText =File.ReadAllText(@"./variant_data/1/ph_2_exp_1.md"),
                                        Effects = new List<Effect>
                                        {
                                            new Effect
                                            {
                                                EffectCategory = EffectCategory.Budget,
                                                Value = +10
                                            }
                                        }
                                    },
                                    new Decision
                                    {
                                        DecisionMarkdownText =  File.ReadAllText(@"./variant_data/1/ph_2_dec_2.md"),
                                        ExplanationMarkdownText =File.ReadAllText(@"./variant_data/1/ph_2_exp_2.md"),
                                        Effects = new List<Effect>
                                        {
                                            new Effect
                                            {
                                                EffectCategory = EffectCategory.Budget,
                                                Value = +10
                                            }
                                        }
                                    },
                                }
                            },
                            new GamePhase
                            {
                                Sequence = 3,
                                Duration = PhaseDuration.Infinite,
                                ProductOwnerMission = new Mission
                                {
                                    MarkdownText = File.ReadAllText(@"./variant_data/1/po_mission_3.md")
                                },
                                StakeholderMissions = new List<Mission>
                                {
                                    new Mission
                                    {
                                        MarkdownText = File.ReadAllText(@"./variant_data/1/sh_mission_5.md")
                                    },
                                     new Mission
                                    {
                                        MarkdownText = File.ReadAllText(@"./variant_data/1/sh_mission_6.md")
                                    },
                                },
                                Decisions = new List<Decision>
                                {
                                    new Decision
                                    {
                                        DecisionMarkdownText =  File.ReadAllText(@"./variant_data/1/ph_3_dec_1.md"),
                                        ExplanationMarkdownText =File.ReadAllText(@"./variant_data/1/ph_3_exp_1.md"),
                                        Effects = new List<Effect>
                                        {
                                            new Effect
                                            {
                                                EffectCategory = EffectCategory.Budget,
                                                Value = +10
                                            }
                                        }
                                    },
                                    new Decision
                                    {
                                       DecisionMarkdownText =  File.ReadAllText(@"./variant_data/1/ph_3_dec_2.md"),
                                        ExplanationMarkdownText =File.ReadAllText(@"./variant_data/1/ph_3_exp_2.md"),
                                        Effects = new List<Effect>
                                        {
                                            new Effect
                                            {
                                                EffectCategory = EffectCategory.Budget,
                                                Value = +10
                                            }
                                        }
                                    },
                                }
                            }
                        }
                    };

                    gvDataAccess.CreateGameVariantAsync(gv).Wait();
                }
                catch (Exception ex)
                {

                }
            }

            // Start game processing
            var gameManager = app.ApplicationServices.GetService<IGameManager>();
            gameManager.Start();
        }
    }
}
