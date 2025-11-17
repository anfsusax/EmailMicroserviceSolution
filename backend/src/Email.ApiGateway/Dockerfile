FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9000
ENV ASPNETCORE_URLS=http://+:9000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Email.ApiGateway/Email.ApiGateway.csproj", "src/Email.ApiGateway/"]
COPY ["src/Email.Application/Email.Application.csproj", "src/Email.Application/"]
COPY ["src/Email.Infrastructure/Email.Infrastructure.csproj", "src/Email.Infrastructure/"]
COPY ["src/Email.Domain/Email.Domain.csproj", "src/Email.Domain/"]
RUN dotnet restore "src/Email.ApiGateway/Email.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Email.ApiGateway"
RUN dotnet publish "Email.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Email.ApiGateway.dll"]

