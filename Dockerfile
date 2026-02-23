
    FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
    WORKDIR /src
    
    COPY SuperMarket.slnx .
    COPY SuperMarket.API/SuperMarket.API.csproj SuperMarket.API/
    COPY SuperMarket.Application/SuperMarket.Application.csproj SuperMarket.Application/
    COPY SuperMarket.Domain/SuperMarket.Domain.csproj SuperMarket.Domain/
    COPY SuperMarket.Infrastructure/SuperMarket.Infrastructure.csproj SuperMarket.Infrastructure/
    
    RUN dotnet restore SuperMarket.API/SuperMarket.API.csproj
    
    COPY . .
    RUN dotnet publish SuperMarket.API/SuperMarket.API.csproj -c Release -o /app/publish
    
    # ---------- Runtime stage ----------
    FROM mcr.microsoft.com/dotnet/aspnet:10.0
    WORKDIR /app
    EXPOSE 3000
    
    COPY --from=build /app/publish .
    
    ENTRYPOINT ["dotnet", "SuperMarket.API.dll"]