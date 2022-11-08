#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ocelot_APIGw/Ocelot_APIGw.csproj", "Ocelot_APIGw/"]
RUN dotnet restore "Ocelot_APIGw/Ocelot_APIGw.csproj"
COPY . .
WORKDIR "/src/Ocelot_APIGw"
RUN dotnet build "Ocelot_APIGw.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ocelot_APIGw.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ocelot_APIGw.dll"]