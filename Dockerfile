FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY . ./

RUN dotnet restore src/Servitium/Servitium.csproj

RUN dotnet publish src/Servitium/Servitium.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080	
ENTRYPOINT ["dotnet", "Servitium.dll"]