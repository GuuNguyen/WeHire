using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace WeHire.API.Configurations
{
    public static class SwaggerConfig
    {
        public static void RegisterSwaggerModule(this IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                // Set Description Swagger
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WeHire.WebAPI",
                    Version = "v1",
                    Description = "WeHire API Endpoints",
                });

                //c.DescribeAllParametersInCamelCase();
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);

                // Set Authorize box to swagger
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your token on textbox below!",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {jwtSecurityScheme, Array.Empty<string>()}
                });

            });
            services.AddSwaggerGenNewtonsoftSupport();
            // return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApplicationSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "{documentName}/api-docs";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/v1/api-docs", "WeHire.WebAPI v1");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}
