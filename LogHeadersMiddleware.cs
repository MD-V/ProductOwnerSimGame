using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProductOwnerSimGame
{
    public class LogHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public LogHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                //string body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            }
            catch(Exception ex)
            {

            }
           
            //Continue down the Middleware pipeline, eventually returning to this class
            await _next(context);

              

        }
    }
}
