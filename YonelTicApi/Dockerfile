# Build aþamasý
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Proje dosyasýný kopyala ve restore et
COPY *.csproj ./
RUN dotnet restore

# Tüm dosyalarý kopyala ve publish et
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime aþamasý
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Uygulamayý baþlat
ENTRYPOINT ["dotnet", "YonelTicApi.dll"]
