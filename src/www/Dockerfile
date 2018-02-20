# https://github.com/dotnet/dotnet-docker-samples/blob/master/aspnetapp/Dockerfile
# https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files/
FROM microsoft/aspnetcore AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001


# copy sln/csproj first for restore, which allows docker to cache results provided these
# files are not changed
FROM microsoft/aspnetcore-build AS build-env
WORKDIR /build
COPY mikeandwan.us.sln                    mikeandwan.us.sln
COPY src/api/api.csproj                   src/api/api.csproj
COPY src/auth/auth.csproj                 src/auth/auth.csproj
COPY src/Maw.Data/Maw.Data.csproj         src/Maw.Data/Maw.Data.csproj
COPY src/Maw.Domain/Maw.Domain.csproj     src/Maw.Domain/Maw.Domain.csproj
COPY src/Maw.Security/Maw.Security.csproj src/Maw.Security/Maw.Security.csproj
COPY src/www/www.csproj                   src/www/www.csproj
RUN dotnet restore


# copy everything else and build
COPY . .
RUN dotnet build --no-restore -c Release -o out /build/src/www/www.csproj
RUN dotnet publish /build/src/www/www.csproj --no-restore -c Release -o out


# build runtime image
FROM base AS production
WORKDIR /app
COPY --from=build-env /build/src/www/out .
ENTRYPOINT [ "dotnet", "maw_www.dll" ]
