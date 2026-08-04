FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PlataformaGestionEventos/PlataformaGestionEventos.csproj", "PlataformaGestionEventos/"]
RUN dotnet restore "PlataformaGestionEventos/PlataformaGestionEventos.csproj"

COPY . .
WORKDIR "/src/PlataformaGestionEventos"

RUN dotnet publish "PlataformaGestionEventos.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PlataformaGestionEventos.dll"]