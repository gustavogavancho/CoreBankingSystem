# .NET SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy only project files first for better cache
COPY CoreBankingSystem.Api/CoreBankingSystem.Api.csproj CoreBankingSystem.Api/
COPY CoreBankingSystem.Application/CoreBankingSystem.Application.csproj CoreBankingSystem.Application/
COPY CoreBankingSystem.Persistence/CoreBankingSystem.Persistence.csproj CoreBankingSystem.Persistence/
COPY CoreBankingSystem.Domain/CoreBankingSystem.Domain.csproj CoreBankingSystem.Domain/

# restore only the API project (avoids test and Angular projects in solution)
RUN dotnet restore CoreBankingSystem.Api/CoreBankingSystem.Api.csproj

# copy the rest of the source
COPY . .

# publish API
WORKDIR /src/CoreBankingSystem.Api
RUN dotnet publish CoreBankingSystem.Api.csproj -c Release -o /app/publish

# ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
# environment variables for ASP.NET
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CoreBankingSystem.Api.dll"]
