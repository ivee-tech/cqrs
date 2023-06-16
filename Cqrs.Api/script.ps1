# from solution folder
# add migration
dotnet ef migrations add Cqrs_Init --project .\Cqrs.Infrastructure\Cqrs.Infrastructure.csproj --startup-project .\Cqrs.Api\Cqrs.Api.csproj

# update DB
dotnet ef database update --project .\Cqrs.Infrastructure\Cqrs.Infrastructure.csproj --startup-project .\Cqrs.Api\Cqrs.Api.csproj


# optionally, remove last migration
dotnet ef migrations remove --project .\Cqrs.Infrastructure\Cqrs.Infrastructure.csproj --startup-project .\Cqrs.Api\Cqrs.Api.csproj
