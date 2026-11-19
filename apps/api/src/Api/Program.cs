using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyDisks.Contracts.Validators;
using MyDisks.DependencyInjection.Configurations;
using MyDisks.RandomServices;
using MyDisks.RetryService;

namespace MyDisks.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApiServices(builder.Configuration);
        builder.Services.RegisterApplicationServices(builder.Configuration);
        builder.Services.AddKeycloakAuthentication(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI();

        // Traefik redirects external HTTP; internal health probes remain HTTP.
        if (!app.Configuration.GetValue<bool>("Https:TerminatedAtProxy"))
            app.UseHttpsRedirection();
        app.UseCors(corsPolicyBuilder =>
            {
                corsPolicyBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }
        );
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/api/auth/me", (HttpContext context) =>
            {
                context.Response.Headers.CacheControl = "no-store";
                return context.User.Claims.Select(claim => new { claim.Type, claim.Value }).ToArray();
            })
            .RequireAuthorization();
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready")
        });

        app.Run();
    }
}
