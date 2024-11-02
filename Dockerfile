FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src
COPY ["justdoit_fixer.csproj", "./"]
COPY ["nuget.config", "./"]
RUN dotnet restore "justdoit_fixer.csproj"
COPY . .
WORKDIR "/src/"
RUN libman restore
RUN dotnet build "justdoit_fixer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "justdoit_fixer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "justdoit_fixer.dll"]
