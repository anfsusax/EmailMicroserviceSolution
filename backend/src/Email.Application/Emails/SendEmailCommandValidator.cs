using System;
using FluentValidation;

namespace Email.Application.Emails;

public sealed class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        RuleFor(c => c.Subject).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Body).NotEmpty();
        RuleFor(c => c.To)
            .NotEmpty()
            .Must(r => r.All(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Address)))
            .WithMessage("Todos os destinatários devem possuir nome e endereço.");
        RuleForEach(c => c.Cc)
            .Must(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Destinatários em cópia devem possuir nome e endereço.")
            .OverridePropertyName("Cc")
            .When(c => c.Cc is not null);
        RuleForEach(c => c.Bcc)
            .Must(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Destinatários ocultos devem possuir nome e endereço.")
            .OverridePropertyName("Bcc")
            .When(c => c.Bcc is not null);
        RuleForEach(c => c.Attachments).SetValidator(new AttachmentInputValidator());
        RuleFor(c => c.Metadata)
            .Must(meta => meta == null || meta.Keys.All(k => !string.IsNullOrWhiteSpace(k)))
            .WithMessage("As chaves de metadata não podem ser vazias.");
    }

    private sealed class AttachmentInputValidator : AbstractValidator<AttachmentInput>
    {
        public AttachmentInputValidator()
        {
            RuleFor(x => x.FileName).NotEmpty();
            RuleFor(x => x.ContentType).NotEmpty();
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Base64Content) || !string.IsNullOrWhiteSpace(x.ExternalUrl))
                .WithMessage("Informe base64Content ou externalUrl para cada anexo.");
        }
    }
}

