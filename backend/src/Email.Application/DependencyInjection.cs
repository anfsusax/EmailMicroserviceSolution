using Email.Application.Configuration;
using Email.Application.Emails;
using Email.Application.Processing;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Email.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailProcessingOptions>(configuration.GetSection(EmailProcessingOptions.SectionName));

        services.AddScoped<IValidator<SendEmailCommand>, SendEmailCommandValidator>();
        services.AddScoped<IEmailCommandHandler, SendEmailCommandHandler>();
        services.AddSingleton<EmailProcessingService>();

        return services;
    }
}

