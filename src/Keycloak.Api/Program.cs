using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.OpenApi.Models;

namespace Keycloak.Api;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddKeycloakAuthentication(new KeycloakAuthenticationOptions()
		{
			// Keycloak server URL
			AuthServerUrl = builder.Configuration["Keycloak:auth-server-url"]!,
			// Realm Name
			Realm = builder.Configuration["Keycloak:realm"]!,
			// ClientId
			Resource = builder.Configuration["Keycloak:resource"]!,

			SslRequired = builder.Configuration["Keycloak:ssl-required"]!,
			VerifyTokenAudience = false,
		});

		builder.Services.AddKeycloakAuthorization(new KeycloakProtectionClientOptions()
		{
			AuthServerUrl = builder.Configuration["Keycloak:auth-server-url"]!,
			Realm = builder.Configuration["Keycloak:realm"]!,
			Resource = builder.Configuration["Keycloak:resource"]!,
			SslRequired = builder.Configuration["Keycloak:ssl-required"]!,
			VerifyTokenAudience = false
		});

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();

		builder.Services.AddSwaggerGen(c =>
		{
			var securityScheme = new OpenApiSecurityScheme
			{
				Name = "Keycloak",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.OpenIdConnect,
				OpenIdConnectUrl = new Uri($"{builder.Configuration["Keycloak:auth-server-url"]}realms/{builder.Configuration["Keycloak:realm"]}/.well-known/openid-configuration"),
				Scheme = "bearer",
				BearerFormat = "JWT",
				Reference = new OpenApiReference
				{
					Id = "Bearer",
					Type = ReferenceType.SecurityScheme
				}
			};
			c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{securityScheme, Array.Empty<string>()}
			});
		});

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseCors(a => a.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

		app.UseAuthentication();
		app.UseAuthorization();


		app.MapControllers();

		app.Run();
	}
}