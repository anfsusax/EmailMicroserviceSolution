using Email.Application.Abstractions;
using Email.Domain.Entities;
using Email.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;
using System.Linq;

namespace Email.Infrastructure.Mail;

internal sealed class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<EmailSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var mimeMessage = BuildMessage(message);
            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
            }

            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return new EmailSendResult(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail {EmailId}", message.Id);
            return new EmailSendResult(false, ex.Message);
        }
    }

    private MimeMessage BuildMessage(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        foreach (var recipient in message.Recipients)
        {
            mimeMessage.To.Add(new MailboxAddress(recipient.Name, recipient.Address.Value));
        }
        foreach (var recipient in message.CcRecipients)
        {
            mimeMessage.Cc.Add(new MailboxAddress(recipient.Name, recipient.Address.Value));
        }
        foreach (var recipient in message.BccRecipients)
        {
            mimeMessage.Bcc.Add(new MailboxAddress(recipient.Name, recipient.Address.Value));
        }

        mimeMessage.Subject = message.Content.Subject;
        var builder = new BodyBuilder
        {
            HtmlBody = message.Content.IsHtml ? message.Content.Body : null,
            TextBody = message.Content.IsHtml ? null : message.Content.Body
        };

        var linkSnippets = new List<(string FileName, Uri Url)>();
        foreach (var attachment in message.Attachments)
        {
            if (attachment.Content is not null)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
            }
            else if (attachment.ExternalUrl is not null)
            {
                linkSnippets.Add((attachment.FileName, attachment.ExternalUrl));
            }
        }

        if (linkSnippets.Count > 0)
        {
            if (message.Content.IsHtml)
            {
                var list = string.Join(string.Empty, linkSnippets.Select(l =>
                    $"<li><a href=\"{l.Url}\">{System.Net.WebUtility.HtmlEncode(l.FileName)}</a></li>"));
                builder.HtmlBody = (builder.HtmlBody ?? string.Empty) + $"<hr/><p>Links seguros:</p><ul>{list}</ul>";
            }
            else
            {
                var linksBlock = string.Join(Environment.NewLine, linkSnippets.Select(l => $"{l.FileName}: {l.Url}"));
                builder.TextBody = (builder.TextBody ?? string.Empty) + $"{Environment.NewLine}Links seguros:{Environment.NewLine}{linksBlock}";
            }
        }

        mimeMessage.Body = builder.ToMessageBody();
        return mimeMessage;
    }
}

