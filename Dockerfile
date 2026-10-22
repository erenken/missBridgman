FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src

COPY MissBridgman.Web/MissBridgman.Web.csproj MissBridgman.Web/
RUN dotnet restore MissBridgman.Web/MissBridgman.Web.csproj

COPY MissBridgman.Web/ MissBridgman.Web/
WORKDIR /src/MissBridgman.Web
RUN dotnet publish MissBridgman.Web.csproj --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MissBridgman.Web.dll"]
