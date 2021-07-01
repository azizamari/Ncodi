FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
EXPOSE 8050

ENV ASPNETCORE_URLS=http://+:80


COPY . ./

RUN dotnet publish Ncodi.Web/Ncodi.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Ncodi.Web.dll"]