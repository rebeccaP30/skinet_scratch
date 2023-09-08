﻿using System.Net;
using System.Text.Json;
using API.Errors;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ILogger<ExceptionMiddleware> Logger { get; }
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            Logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                context.Response.ContentType = "applicatiom/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                    ? new ApiException ((int)HttpStatusCode.InternalServerError, ex.Message,ex.StackTrace.ToString())
                    : new ApiResponse((int)HttpStatusCode.InternalServerError);
                    
                    var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                    var json = JsonSerializer.Serialize(response);

                    await context.Response.WriteAsync(json);
                    




            }
        }
    }
}