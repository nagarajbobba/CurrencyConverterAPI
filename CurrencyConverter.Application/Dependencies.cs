using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Application.Currency.Validators;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Application
{
    [ExcludeFromCodeCoverage]
    public static class Dependencies
    {
        public static IServiceCollection RegisterApplicationDependencies(this IServiceCollection services)
        {
            services.AddMediatR(typeof(GetLatestRatesQuery).Assembly);
            services.AddTransient<IValidator<ConvertCurrencyQuery>, ConvertCurrencyQueryValidator>();
            services.AddTransient<IValidator<GetLatestRatesQuery>, GetLatestRatesQueryValidator>();
            services.AddTransient < IValidator <GetConversionHistoryQuery>, GetConversionHistoryQueryValidator>();
            //services.AddMediatR(typeof(Dependencies).Assembly);
            //services.AddValidatorsFromAssembly(typeof(Dependencies).Assembly);
            // services.AddMediatR((typeof(ConvertCurrencyQuery).Assembly));

            // Register FluentValidation
            //   services.AddValidatorsFromAssemblyContaining<ConvertCurrencyQueryValidator>();

            // Register MediatR
            //  services.AddMediatR(typeof(ConvertCurrencyQueryValidator).Assembly);

            return services;
        }
    }
}
