# Stage 1: Base slim runtime layer
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: SDK compilation engine layer
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project definition files to cache restore steps
COPY ProjectIntern/ProjectIntern.csproj ProjectIntern/
COPY ProjectIntern.Data/ProjectIntern.Data.csproj ProjectIntern.Data/
COPY ProjectIntern.Data.Models/ProjectIntern.Data.Models.csproj ProjectIntern.Data.Models/
COPY ProjectIntern.Data.Seeding/ProjectIntern.Data.Seeding.csproj ProjectIntern.Data.Seeding/
COPY ProjectIntern.Services.Core/ProjectIntern.Services.Core.csproj ProjectIntern.Services.Core/
COPY ProjectIntern.Web.ViewModels/ProjectIntern.Web.ViewModels.csproj ProjectIntern.Web.ViewModels/
COPY ProjectIntern.GCommon/ProjectIntern.GCommon.csproj ProjectIntern.GCommon/

RUN dotnet restore "./ProjectIntern/ProjectIntern.csproj"
COPY . .

# Compile the target solution assemblies
WORKDIR "/src/ProjectIntern"
RUN dotnet build "./ProjectIntern.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish distribution bundles
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjectIntern.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Production execution image constructor
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#Enforce UTC time zones natively inside the container framework for PostgreSQL
ENV TZ=UTC
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ProjectIntern.dll"]