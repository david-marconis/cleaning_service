FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS builder
WORKDIR /app/CleaningService.Api

COPY ./CleaningService.Api/CleaningService.Api.csproj ./

RUN dotnet restore

COPY . /app/
RUN dotnet publish CleaningService.Api.csproj -c Release -o publish --no-restore

RUN apk add --no-cache icu-libs tzdata sqlite

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY ./CleaningService.Api/database_schema.sql ./
RUN sqlite3 -init database_schema.sql ../cleaning_service.db .quit
RUN cp appsettings.json ../

WORKDIR /app
ENTRYPOINT ["dotnet"]
CMD ["/app/CleaningService.Api/publish/CleaningService.Api.dll"]
