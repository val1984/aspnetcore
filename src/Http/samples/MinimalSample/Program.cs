// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/hello/{name}", (string name) => $"Hello {name}!")
    .AddEndpointFilterFactory((context, next) =>
    {
        var parameters = context.MethodInfo.GetParameters();
        // Only operate handlers with a single argument
        if (parameters.Length == 1 &&
            parameters[0] is ParameterInfo parameter &&
            parameter.ParameterType == typeof(string))
        {
            return invocationContext =>
            {
                // Map the first string argument we
                // receive to an upper-case string
                var modifiedArgument = invocationContext
                    .GetArgument<string>(0)
                    .ToUpperInvariant();
                invocationContext.Arguments[0] = modifiedArgument;
                return next(invocationContext);
            };
        }

        return invocationContext => next(invocationContext);
    });

app.Run();
