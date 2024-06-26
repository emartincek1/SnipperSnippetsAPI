﻿using SnipperSnippetsAPI.Entities;
using SnipperSnippetsAPI.Repositories.Contracts;
using System.Text;

namespace SnipperSnippetsAPI.Middleware
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IIdentityServiceRepository _identityService)
        {
            string? authHeader = context.Request.Headers["Authorization"];
            if (context.Request.Path.Value is null) return;
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                // Parse username and password from authorization header
                var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                var separator = decodedUsernamePassword.IndexOf(':');
                var username = decodedUsernamePassword.Substring(0, separator);
                var password = decodedUsernamePassword.Substring(separator + 1);

                User? user;

                if ((context.Request.Method == "POST") && !context.Request.Path.Value.StartsWith("/api/users/login"))
                {
                    // If a CreateUser request, use identity service to create a user
                    user = await _identityService.CreateUser(username, password);
                }
                else
                {
                    // Use identity service to authenticate the user
                    user = await _identityService.AuthenticateUser(username, password);
                }
                // If null, the authentication or creation failed
                if (user == null)
                {
                    if (context.Request.Method == "GET")
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized: user doesn't exist or incorrect password");
                        return;
                    }
                    // if POST request, the user creation failed
                    else if (context.Request.Method == "POST" && context.Request.Path.Value.StartsWith("/api/users/login"))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized: user login failed");
                        return;
                    }
                    else if (context.Request.Method == "POST")
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized: user account creation failed");
                        return;
                    }
                }

                    // Add the user information to HttpContext Items so it can be used in the controller
                    context.Items.Add("User", user);
            }

            // Continue the pipeline
            await _next(context);
        }
    }
}
