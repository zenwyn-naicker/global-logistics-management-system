# Author: Zenwyn Naicker
# Code Attribution:
##############################################################################################################
# Author: Microsoft
# Date: 2025
# Title: Run an ASP.NET Core app in Docker containers
# Type: [Source Code]
# Available at: <https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-10.0> [Accessed 18 May 2026]
##############################################################################################################

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GLMS.Web/GLMS.Web.csproj", "GLMS.Web/"]
RUN dotnet restore "GLMS.Web/GLMS.Web.csproj"

COPY . .
WORKDIR "/src/GLMS.Web"
RUN dotnet publish "GLMS.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GLMS.Web.dll"]
