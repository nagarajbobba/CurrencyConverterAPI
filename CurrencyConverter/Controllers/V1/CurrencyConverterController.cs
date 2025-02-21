using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CurrencyConverter.API.Controllers.V1
{
    //[Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrencyConverterController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("latestrates")]
        [Authorize(Roles = "Admin,User,Viewer")] // (RBAC) 
        [SwaggerOperation(Summary = "Gets latest exchange rates", Description = "Gets latest exchange rates")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Success", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Rates are not available", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error occured while processing.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to access this resource.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Permission Denied.")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
        {
                var response = await _mediator.Send(new GetLatestRatesQuery { BaseCurrency = baseCurrency });
                if (response is null)
                {
                    return BadRequest();
                }
                return Ok(response);
        }

        [HttpGet("convert")]
        [Authorize(Roles = "Admin,User")] // (RBAC) 
        [SwaggerOperation(Summary = "Converts currency", Description = "Converts currency")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Success", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Rates are not available", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error occured while processing.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to access this resource.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Permission Denied.")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string baseCurrency, [FromQuery] string targetCurrency, [FromQuery] decimal amount)
        {            
                var request = new CurrencyConversionRequest { BaseCurrency = baseCurrency, TargetCurrency = targetCurrency, Amount = amount };

                var response = await _mediator.Send(new ConvertCurrencyQuery { ConversionRequest = request });
               
                return Ok(response);            
        }

        [HttpGet("history")]
        [Authorize(Roles = "Admin,User")] // (RBAC) 
        [SwaggerOperation(Summary = "Gets currency conversion history", Description = "Gets currency conversion history")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Success", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Rates are not available", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error occured while processing.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to access this resource.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Permission Denied.")]
        public async Task<IActionResult> GetConversionHistory([FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] string baseCurrency, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
             var request = new CurrencyRatesHistoryRequest { BaseCurrency = baseCurrency, StartDate = startDate, EndDate = endDate, Page = page, PageSize = pageSize };

                var response = await _mediator.Send(new GetConversionHistoryQuery { HistoryRequest = request });
                if (response is null)
                {
                    return BadRequest();
                }
                return Ok(response);           
        }
    }
}
