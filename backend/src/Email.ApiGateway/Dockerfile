FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9000
ENV ASPNETCORE_URLS=http://+:9000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EmailMicroserviceSolution.sln", "./"]
COPY ["src/Email.ApiGateway/Email.ApiGateway.csproj", "src/Email.ApiGateway/"]
RUN dotnet restore "EmailMicroserviceSolution.sln"
COPY . .
WORKDIR "/src/src/Email.ApiGateway"
RUN dotnet publish "Email.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Email.ApiGateway.dll"]

