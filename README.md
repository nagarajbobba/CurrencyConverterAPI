# Currency Converter API

A .NET Web API service that provides currency conversion functionality using the Frankfurter API as the data source.

## Setup Instructions

1. Clone the repository:
```sh
git clone https://github.com/nagarajbobba/CurrencyConverterAPI.git
```

2. Prerequisites:
- .NET 8.0 SDK
- Docker (optional)
- Redis Server

3. Configuration:
- Update Redis connection strings in `appsettings.json` or use environment variables:
  - Redis__ConnectionString
  - Redis__ConnectionString_RD
  - Redis__ApplicationCode
  - Redis__Environment
  - Redis__UseReadReplica

4. Run using Docker:
```sh
docker-compose up
```

Or run locally:
```sh
dotnet restore
dotnet build
dotnet run --project CurrencyConverter/CurrencyConverter.API.csproj
```

## Features

- Currency conversion between different currencies
- Historical exchange rates
- Latest exchange rates
- API versioning support
- JWT authentication
- Rate limiting
- Redis caching
- OpenTelemetry integration
- Circuit breaker pattern for resilient API calls
- Serilog seq logging

## Assumptions

1. Frankfurter API is the primary data source for exchange rates
2. Redis cache is used to improve performance
3. JWT authentication is required for all endpoints
4. API versioning is needed for future compatibility

## Future Enhancements

1. Add support for more currency data providers
2. Enhance caching strategy with different TTLs for different endpoints
3. Add more comprehensive monitoring and alerting
4. Add support for bulk currency conversions
5. Implement user management and role-based access control

## Testing

Run the test suite:
```sh
dotnet test
```

## API Documentation

Access Swagger documentation at:
```
http://localhost:5299/swagger/index.html
```

## Logging

Logs are written to:
- Console
- Seq server (http://localhost:5341)
- Daily rolling file logs in the `logs` directory