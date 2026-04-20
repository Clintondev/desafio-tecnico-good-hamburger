FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src ./src

WORKDIR /src/src/GoodHamburger.Web
RUN dotnet restore GoodHamburger.Web.csproj
RUN dotnet publish GoodHamburger.Web.csproj -c Release -o /app/publish

FROM nginx:1.27-alpine AS final
COPY src/GoodHamburger.Web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

EXPOSE 80
