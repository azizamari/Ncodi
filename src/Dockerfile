FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
EXPOSE 80
EXPOSE 443


COPY . ./

RUN dotnet publish Ncodi.Web/Ncodi.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
RUN adduser \
    --disabled-password \
    --home /app \
    --gecos '' app \
    && chown -R app /app
USER app
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Ncodi.Web.dll"]