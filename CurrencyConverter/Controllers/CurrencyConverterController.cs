using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CurrencyConverter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize()]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrencyConverterController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("latestrates")]
        [SwaggerOperation(Summary = "Gets latest exchange rates", Description = "Gets latest exchange rates")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Success", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Rates are not available", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error occured while processing.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to access this resource.")]
        //[SwaggerResponse(StatusCodes.Status403Forbidden, AppConstants.ErrorMessages.PermissionDeniedError)]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency))
                return BadRequest("Base currency is required");
           var response = await _mediator.Send(new GetLatestRatesQuery { BaseCurrency = baseCurrency });
           
            return Ok(response);
        }

        [HttpGet("convert")]
        [SwaggerOperation(Summary = "Converts currency", Description = "Converts currency")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Success", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Rates are not available", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error occured while processing.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to access this resource.")]
        //[SwaggerResponse(StatusCodes.Status403Forbidden, AppConstants.ErrorMessages.PermissionDeniedError
        public async Task<IActionResult> ConvertCurrency([FromQuery] string baseCurrency, [FromQuery] string targetCurrency, [FromQuery] decimal amount)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency) || string.IsNullOrWhiteSpace(targetCurrency))
                return BadRequest("Base currency and target currency are required");
            var request = new CurrencyConversionRequest { BaseCurrency = baseCurrency, TargetCurrency = targetCurrency, Amount = amount };
            var response = await _mediator.Send(new ConvertCurrencyQuery { ConversionRequest = request });
            return Ok(response);
        }

        [HttpGet("history")]
        [SwaggerOperation(Summary = "Gets currency conversion history", Description = "Gets currency conversion history")]
        [SwaggerResponse(StatusCodes.Status200OK, "Request Success", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Rates are not available", typeof(CurrencyRates))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error occured while processing.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to access this resource.")]
        //[SwaggerResponse(StatusCodes.Status403Forbidden, AppConstants.ErrorMessages.PermissionDeniedError
        public async Task<IActionResult> GetConversionHistory([FromQuery] string startDate, [FromQuery] string endDate, [FromQuery] string baseCurrency, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency))
                return BadRequest("Base currency is required");
            var request = new CurrencyRatesHistoryRequest { BaseCurrency = baseCurrency, StartDate = startDate, EndDate = endDate, Page = page, PageSize = pageSize };
            var response = await _mediator.Send(new GetConversionHistoryQuery { HistoryRequest = request });
           
            return Ok(response);
        }
    }
}
