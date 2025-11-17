using Email.Application.Abstractions;
using Email.Infrastructure.Configuration;
using Email.Infrastructure.Mail;
using Email.Infrastructure.Messaging;
using Email.Infrastructure.Observability;
using Email.Infrastructure.Persistence;
using Email.Infrastructure.Templates;
using Email.Infrastructure.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Email.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.Configure<EmailTemplateOptions>(configuration.GetSection(EmailTemplateOptions.SectionName));

        services.AddSingleton<RabbitMqEmailQueue>();
        services.AddSingleton<IEmailQueueProducer>(sp => sp.GetRequiredService<RabbitMqEmailQueue>());
        services.AddSingleton<IEmailQueueConsumer>(sp => sp.GetRequiredService<RabbitMqEmailQueue>());
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddSingleton<IEmailStatusStore, InMemoryEmailStatusStore>();
        services.AddSingleton<IMetricsPublisher, EmailMetricsPublisher>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<ITemplateRenderer, InMemoryTemplateRenderer>();

        return services;
    }
}

