FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

EXPOSE 5093
EXPOSE 80 
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . /src
WORKDIR /src
RUN dotnet build "FormApp/FormApp.csproj" -c Debug -o /app/build

FROM build as publish
RUN dotnet publish "FormApp/FormApp.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FormApp.dll"]
