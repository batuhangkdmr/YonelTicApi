# Build a�amas�
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Proje dosyas�n� kopyala ve restore et
COPY *.csproj ./
RUN dotnet restore

# T�m dosyalar� kopyala ve publish et
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime a�amas�
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Uygulamay� ba�lat
ENTRYPOINT ["dotnet", "YonelTicApi.dll"]
