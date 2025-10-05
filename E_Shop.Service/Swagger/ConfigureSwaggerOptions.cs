using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace E_Shop.Service.Swagger;

public class ConfigureSwaggerOptions() : IConfigureOptions<SwaggerGenOptions>
{

    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "E-Shop API",
            Version = "v1",
            Description = "E-Shop API for managing products, users, and authentication",
            Contact = new OpenApiContact
            {
                Name = "E-Shop Team"
            }
        });

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Please add your token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        });

        // Ensure OpenAPI 3.0 specification
        options.UseInlineDefinitionsForEnums();
        options.SupportNonNullableReferenceTypes();
    }

}