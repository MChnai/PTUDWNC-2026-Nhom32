FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/CulinaryBlog.Domain/*.csproj src/CulinaryBlog.Domain/
COPY src/CulinaryBlog.Application/*.csproj src/CulinaryBlog.Application/
COPY src/CulinaryBlog.Infrastructure/*.csproj src/CulinaryBlog.Infrastructure/
COPY src/CulinaryBlog.API/*.csproj src/CulinaryBlog.API/
RUN dotnet restore src/CulinaryBlog.API/CulinaryBlog.API.csproj
COPY . .
RUN dotnet publish src/CulinaryBlog.API/CulinaryBlog.API.csproj -c Release -o /out --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends libgomp1 && rm -rf /var/lib/apt/lists/*
COPY --from=build /out .
RUN mkdir -p /app/wwwroot/uploads && chown -R app:app /app/wwwroot
USER app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "CulinaryBlog.API.dll"]
