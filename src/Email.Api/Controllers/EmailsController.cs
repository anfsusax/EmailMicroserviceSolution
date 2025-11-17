using Email.Api.Contracts;
using Email.Application.Abstractions;
using Email.Application.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Email.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmailsController : ControllerBase
{
    private readonly IEmailCommandHandler _handler;
    private readonly IEmailStatusStore _statusStore;

    public EmailsController(IEmailCommandHandler handler, IEmailStatusStore statusStore)
    {
        _handler = handler;
        _statusStore = statusStore;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(EmailResponseDto), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendAsync([FromBody] SendEmailRequest request, CancellationToken cancellationToken)
    {
        var response = await _handler.HandleAsync(request.ToCommand(), cancellationToken);
        return AcceptedAtRoute("GetEmailStatus", new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}", Name = "GetEmailStatus")]
    [Authorize]
    [ProducesResponseType(typeof(EmailStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatusAsync(Guid id, CancellationToken cancellationToken)
    {
        var message = await _statusStore.GetAsync(id, cancellationToken);
        if (message is null)
        {
            return NotFound();
        }

        return Ok(new EmailStatusResponse(message.Id, message.Status, message.FailureReason));
    }
}

