# -----------------------------
# STEP 1: Build the application
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ../ShopThueBanSach.sln ./
COPY ./ShopThueBanSach.Server/*.csproj ./ShopThueBanSach.Server/

# Restore NuGet packages
RUN dotnet restore

# Copy the full source code
COPY . .

# Publish the application
RUN dotnet publish ./ShopThueBanSach.Server.csproj -c Release -o /app/publish

# -----------------------------
# STEP 2: Runtime image
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port your app runs on
EXPOSE 5000

# Start the application
ENTRYPOINT ["dotnet", "ShopThueBanSach.Server.dll"]