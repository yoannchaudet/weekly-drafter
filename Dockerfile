# Build a small image for .NET 7
# Largely inspired by https://www.thorsten-hans.com/how-to-build-smaller-and-secure-docker-images-for-net5

#
# Publish
#
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS publish
ARG RUNTIME=alpine-x64
WORKDIR /src
COPY . .
RUN ls -la && pwd
RUN dotnet restore --runtime $RUNTIME
RUN dotnet publish WeeklyDrafter/WeeklyDrafter.csproj -c Release -o /app/publish \
  --no-restore \
  --runtime $RUNTIME \
  --self-contained true \
  /p:PublishSingleFile=true

#
# Final
#
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine AS final
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
RUN apk upgrade musl
USER dotnetuser
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/WeeklyDrafter"]