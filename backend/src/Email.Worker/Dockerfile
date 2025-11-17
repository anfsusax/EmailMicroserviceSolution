FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Email.Worker/Email.Worker.csproj", "src/Email.Worker/"]
COPY ["src/Email.Application/Email.Application.csproj", "src/Email.Application/"]
COPY ["src/Email.Infrastructure/Email.Infrastructure.csproj", "src/Email.Infrastructure/"]
COPY ["src/Email.Domain/Email.Domain.csproj", "src/Email.Domain/"]
RUN dotnet restore "src/Email.Worker/Email.Worker.csproj"
COPY . .
WORKDIR "/src/src/Email.Worker"
RUN dotnet publish "Email.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Email.Worker.dll"]

