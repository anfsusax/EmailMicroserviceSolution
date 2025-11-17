using System.Collections.Generic;
using Email.Domain.Entities;
using Email.Domain.ValueObjects;

namespace Email.Domain.Tests;

public class EmailMessageTests
{
    [Fact]
    public void Create_ShouldPopulateFields()
    {
        var content = EmailContent.Create("Assunto", "Corpo", true);
        var recipient = Recipient.Create("Cliente", "cliente@empresa.com");

        var email = EmailMessage.Create(content, new[] { recipient });

        Assert.Equal("Assunto", email.Content.Subject);
        Assert.Single(email.Recipients);
    }

    [Fact]
    public void MarkFailed_ShouldIncreaseRetry()
    {
        var content = EmailContent.Create("Assunto", "Corpo", true);
        var recipient = Recipient.Create("Cliente", "cliente@empresa.com");
        var email = EmailMessage.Create(content, new[] { recipient });

        email.MarkFailed("erro", canRetry: true);

        Assert.Equal(1, email.RetryCount);
    }

    [Fact]
    public void Create_ShouldStoreCcBccAndMetadata()
    {
        var content = EmailContent.Create("Assunto", "Corpo", true);
        var to = Recipient.Create("Cliente", "cliente@empresa.com");
        var cc = Recipient.Create("Financeiro", "fin@empresa.com");
        var bcc = Recipient.Create("Auditoria", "audit@empresa.com");

        var email = EmailMessage.Create(
            content,
            new[] { to },
            attachments: null,
            ccRecipients: new[] { cc },
            bccRecipients: new[] { bcc },
            metadata: new Dictionary<string, string> { ["prioridade"] = "alta" });

        Assert.Single(email.CcRecipients);
        Assert.Single(email.BccRecipients);
        Assert.Equal("alta", email.Metadata["prioridade"]);
    }

    [Fact]
    public void ShouldProcess_ShouldRespectSchedule()
    {
        var content = EmailContent.Create("Assunto", "Corpo", true);
        var recipient = Recipient.Create("Cliente", "cliente@empresa.com");
        var future = DateTimeOffset.UtcNow.AddHours(1);
        var email = EmailMessage.Create(content, new[] { recipient }, scheduledFor: future);

        Assert.False(email.ShouldProcess(DateTimeOffset.UtcNow));
    }
}

