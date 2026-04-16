FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ProfileApi.csproj", "."]
RUN dotnet restore "./ProfileApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ProfileApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProfileApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Ensure SQLite can write to disk
RUN mkdir -p /app/data && chmod 777 /app/data
ENTRYPOINT ["dotnet", "ProfileApi.dll"]