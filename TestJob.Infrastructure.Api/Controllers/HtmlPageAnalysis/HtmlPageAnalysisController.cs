using System.ComponentModel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TestJob.Application.Domains.Requests.HtmlPageAnalysis;
using TestJob.Application.Domains.Responses.HtmlPageAnalysis;

namespace TestJob.Infrastructure.Api.Controllers.HtmlPageAnalysis;

[ApiController]
[Route("/htmlPageAnalysis/")]
[DisplayName("Анализ HTML страницы")]
[Produces("application/json")]
public class HtmlPageAnalysisController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// HtmlPageAnalysisController
    /// </summary>
    /// <param name="mediator"></param>
    public HtmlPageAnalysisController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    /// <summary>
    /// Анализировать страницу
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("analyzePage")]
    [SwaggerResponse(StatusCodes.Status200OK, "Анализировать страницу", typeof(AnalyzePageResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Анализировать страницу", typeof(AnalyzePageResponse))]
    public async Task<IActionResult> AnalyzePage([FromBody] AnalyzePageRequest request)
    {
        var resp = await _mediator.Send(request);

        if (resp.Is_error == 0)
            return Ok(resp);
        else
            return BadRequest(resp);
    }
}