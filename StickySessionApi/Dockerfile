FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/StickySessionApi.Domain/StickySessionApi.Domain.csproj src/StickySessionApi.Domain/
COPY src/StickySessionApi.Application/StickySessionApi.Application.csproj src/StickySessionApi.Application/
COPY src/StickySessionApi.Infrastructure/StickySessionApi.Infrastructure.csproj src/StickySessionApi.Infrastructure/
COPY src/StickySessionApi.Api/StickySessionApi.Api.csproj src/StickySessionApi.Api/
RUN dotnet restore src/StickySessionApi.Api/StickySessionApi.Api.csproj

COPY src/ src/
RUN dotnet publish src/StickySessionApi.Api/StickySessionApi.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "StickySessionApi.Api.dll"]
