using CurrencyConverter.Application.Currency.Queries;
using FluentValidation;

namespace CurrencyConverter.Application.Currency.Validators
{
    public class GetConversionHistoryQueryValidator : AbstractValidator<GetConversionHistoryQuery>
    {
        public GetConversionHistoryQueryValidator()
        {
            RuleFor(x => x.HistoryRequest).NotNull();
            RuleFor(x => x.HistoryRequest.BaseCurrency).NotEmpty();          
            RuleFor(x => x.HistoryRequest.StartDate).NotEmpty();
            RuleFor(x => x.HistoryRequest.EndDate).NotEmpty();            
            RuleFor(x => x.HistoryRequest.Page).GreaterThan(0);
            RuleFor(x => x.HistoryRequest.PageSize).GreaterThan(0);           
            RuleFor(x => x.HistoryRequest.StartDate)
                .LessThan(x => x.HistoryRequest.EndDate)
                .WithMessage("Start date must be before end date.");
        }
    }
}
