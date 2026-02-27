# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Tüm dosyaları kopyala
COPY . .

# Restore + Publish tek adımda
RUN dotnet publish UrlShortener.API/UrlShortener.API.csproj \
    -c Release \
    -o /app/publish

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]