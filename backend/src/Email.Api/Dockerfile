FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EmailMicroserviceSolution.sln", "./"]
COPY ["src/Email.Api/Email.Api.csproj", "src/Email.Api/"]
COPY ["src/Email.Application/Email.Application.csproj", "src/Email.Application/"]
COPY ["src/Email.Infrastructure/Email.Infrastructure.csproj", "src/Email.Infrastructure/"]
COPY ["src/Email.Domain/Email.Domain.csproj", "src/Email.Domain/"]
RUN dotnet restore "EmailMicroserviceSolution.sln"
COPY . .
WORKDIR "/src/src/Email.Api"
RUN dotnet publish "Email.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Email.Api.dll"]

