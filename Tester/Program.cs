using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using ProductOwnerSimGame.Dtos.GameView;
using ProductOwnerSimGame.Dtos.Requests.Game;
using ProductOwnerSimGame.Dtos.Requests.Organization;
using ProductOwnerSimGame.Dtos.Response.Game;
using ProductOwnerSimGame.Dtos.Response.GameVariant;
using ProductOwnerSimGame.Dtos.Response.Organization;
using ProductOwnerSimGame.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tester
{
    public class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            var initialMissionWait = new AutoResetEvent(false);

            var missionWait = new AutoResetEvent(false);

            var decisionWait = new AutoResetEvent(false);

            var gamefinished = false;

            MissionView missionView = null;
            DecisionView decisionView = null;
            
            
            client.BaseAddress = new Uri("https://localhost:5001/api/");

            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/game-view-updates")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            try
            {
                connection.On<GameView>("UpdateGameViewAsync", (gameView) =>
                {
                    var x = JsonConvert.SerializeObject(gameView);
                    //Console.WriteLine(x);

                    if(gameView.MissionView != null)
                    {
                        missionView = gameView.MissionView;
                        missionWait.Set();
                       
                    }
                    else if(gameView.InitialMissionView != null)
                    {
                        initialMissionWait.Set();
                        
                    }
                    else if (gameView.DecisionView != null)
                    {
                        decisionView = gameView.DecisionView;
                        decisionWait.Set();
                        
                    }
                    else if(gameView.GameFinishedView != null)
                    {
                        gamefinished = true;
                        missionWait.Set();
                    }


                });

                await connection.StartAsync();

                // Create Organization
                var orgId = await CreateOrganizationAsync("Open IIoT Machines Platform GmbH").ConfigureAwait(false);
                Console.WriteLine("Organization created.");

                // Create Users 

                
                Console.WriteLine("Start creating users.");
                var adminId = await CreateUserAsync("Admin", "MasterAdmin", "admin@admin.com").ConfigureAwait(false);
                Console.WriteLine("     Admin created.");
                var user1Id = await CreateUserAsync("User 1", "User1", "User1@iiot.com").ConfigureAwait(false);
                Console.WriteLine("     User1 created.");
                var user2Id = await CreateUserAsync("User 2", "User2", "User2@iiot.com").ConfigureAwait(false);
                Console.WriteLine("     User2 created.");
                var user3Id = await CreateUserAsync("User 3", "User3", "User3@iiot.com").ConfigureAwait(false);
                Console.WriteLine("     User3 created.");
                var gameMasterId = await CreateUserAsync("GameMaster 1", "GameMaster1", "GameMaster1@iiot.com").ConfigureAwait(false);
                Console.WriteLine("     Game Master created.");
                Console.WriteLine("Users created.");
                

                // Assign Users 
                Console.WriteLine("Start adding users to org.");
                await AddUserToOrganization(orgId, user1Id).ConfigureAwait(false);
                await AddUserToOrganization(orgId, user2Id).ConfigureAwait(false);
                await AddUserToOrganization(orgId, user3Id).ConfigureAwait(false);
                await AddUserToOrganization(orgId, gameMasterId).ConfigureAwait(false);
                Console.WriteLine("Users added to org.");

                // Create GameVariant
                var gameVariant = await CreateGameVariantAsync().ConfigureAwait(false);
                Console.WriteLine("GameVariant created.");

                // Create Game
                var game = await CreateGameAsync(gameVariant, gameMasterId, orgId).ConfigureAwait(false);
                Console.WriteLine("Game created.");

                // Add users to Game
                var adduser1 = await AddUserToGameAsync(game.GameId, game.AccessCode, user1Id).ConfigureAwait(false);
                var adduser2 = await AddUserToGameAsync(game.GameId, game.AccessCode, user2Id).ConfigureAwait(false);
                //var adduser3 = await AddUserToGameAsync(game.GameId, game.AccessCode, user3Id).ConfigureAwait(false);
                Console.WriteLine("Added users to game.");

                // Start game
                var startGame = await StartGamesAsync(game.GameId).ConfigureAwait(false);
                Console.WriteLine("Game started.");

                initialMissionWait.WaitOne();
                initialMissionWait.Reset();
                Console.WriteLine("Initial mission received.");

                // Play game 
                await Task.Delay(TimeSpan.FromSeconds(1));
                await StartPhaseClickedAsync(game.GameId, user1Id).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(1));
                await StartPhaseClickedAsync(game.GameId, user2Id).ConfigureAwait(false);
                Console.WriteLine("Game started by players.");

                while (!gamefinished)
                {
                    Console.WriteLine("Waiting for mission.");
                    missionWait.WaitOne();
                    missionWait.Reset();
                    Console.WriteLine("Mission received.");

                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await StartPhaseClickedAsync(game.GameId, user1Id).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await StartPhaseClickedAsync(game.GameId, user2Id).ConfigureAwait(false);
                    Console.WriteLine("Started Phase.");

                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await PhaseDoneClickedAsync(game.GameId, user2Id).ConfigureAwait(false);
                    Console.WriteLine("Submitted phase done");

                    Console.WriteLine("Waiting for decision view.");
                    decisionWait.WaitOne();
                    decisionWait.Reset();
                    Console.WriteLine("decision view received");

                    var dec = decisionView.Decisions.FirstOrDefault();
                    
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await SubmitDecisionAsync(game.GameId, dec.DecisionId).ConfigureAwait(false);
                    Console.WriteLine("Submitted decision");

                    await Task.Delay(TimeSpan.FromSeconds(1));

                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private static async Task<string> CreateOrganizationAsync(string organization)
        {
            try
            {
                var request = new CreateOrganizationRequest()
                {
                    DisplayName = organization
                };


                HttpResponseMessage response = await client.PostAsync("Organization", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                var responseObj = await response.Content.ReadAsAsync<CreateOrganizationResponse>();

                return responseObj.OrganizationId;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return string.Empty;
        }

        private static async Task<string> CreateGameVariantAsync()
        {
            try
            {

                HttpResponseMessage response = await client.PostAsync("GameVariant", null);
                response.EnsureSuccessStatusCode();
                var responseObj = await response.Content.ReadAsAsync<CreateGameVariantResponse>();

                return responseObj.GameVariantId;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return string.Empty;
        }


        public static async Task<bool> AddUserToOrganization(string organizationId, string userId)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync($"Organization/{organizationId}/adduser/{userId}", null);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return false;
        }

        public static async Task<bool> AddUserToGameAsync(string gameId, string accessCode, string userId)
        {
            try
            {
                var request = new AddUserToGameRequest()
                {
                    AccessCode = accessCode,
                    GameId = gameId,
                    UserId = userId
                };

                HttpResponseMessage response = await client.PostAsync($"Game/AddUser", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return false;
        }

        public static async Task<bool> PhaseDoneClickedAsync(string gameId, string userId)
        {
            try
            {
                var request = new PhaseDoneClickedRequest()
                {
                    GameId = gameId,
                    UserId = userId
                };

                HttpResponseMessage response = await client.PostAsync($"Game/PhaseDoneClicked", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return false;
        }

        public static async Task<bool> StartPhaseClickedAsync(string gameId, string userId)
        {
            try
            {
                var request = new StartPhaseClickedRequest()
                {
                    GameId = gameId,
                    UserId = userId
                };

                HttpResponseMessage response = await client.PostAsync($"Game/StartPhaseClicked", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return false;
        }

        public static async Task<bool> SubmitDecisionAsync(string gameId, string decisionId)
        {
            try
            {
                var request = new SubmitDecisionRequest()
                {
                    GameId = gameId,
                    DecisionId = decisionId
                };

                HttpResponseMessage response = await client.PostAsync($"Game/SubmitDecision", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return false;
        }


        public static async Task<bool> StartGamesAsync(string gameId)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync($"Game/{gameId}/Start", null);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return false;
        }


        private static async Task<string> CreateUserAsync(string name, string displayName, string email)
        {
            //TODO

            return Guid.NewGuid().ToString();
            
            /*
            try
            {
                var request = new CreateEditUserRequest()
                {
                    //Name = name,
                    //DisplayName = displayName,
                    //Role = role,
                    //Email = email
                };


                HttpResponseMessage response = await client.PostAsync("User", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                var responseObj = await response.Content.ReadAsAsync<CreateUserResponse>();

                return responseObj.UserId;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return string.Empty;
            */
        }

        private static async Task<CreateGameResponse> CreateGameAsync(string gameVariantId, string gameMasterUserId, string organizationId)
        {
            try
            {
                var request = new CreateGameRequest()
                {
                    GameVariantId = gameVariantId,
                    GameMasterUserId = gameMasterUserId,
                    OrganizationId = organizationId

                };

                HttpResponseMessage response = await client.PostAsync("Game", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                var responseObj = await response.Content.ReadAsAsync<CreateGameResponse>();

                return responseObj;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            return null;
        }

    }
}
