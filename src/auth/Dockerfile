# https://github.com/dotnet/dotnet-docker-samples/blob/master/aspnetapp/Dockerfile
# https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files/
FROM microsoft/aspnetcore AS base
WORKDIR /app
EXPOSE 5001

# build env is ordered to try and maximize caching for faster builds
FROM microsoft/aspnetcore-build AS build-env
RUN apt-get update && \
    apt-get install -y rsync && \
    rm -rf /var/lib/apt/lists/*
WORKDIR /build

# copy sln/csproj first for restore
COPY mikeandwan.us.sln                    mikeandwan.us.sln
COPY src/api/api.csproj                   src/api/api.csproj
COPY src/auth/auth.csproj                 src/auth/auth.csproj
COPY src/Maw.Data/Maw.Data.csproj         src/Maw.Data/Maw.Data.csproj
COPY src/Maw.Domain/Maw.Domain.csproj     src/Maw.Domain/Maw.Domain.csproj
COPY src/Maw.Security/Maw.Security.csproj src/Maw.Security/Maw.Security.csproj
COPY src/www/www.csproj                   src/www/www.csproj
RUN dotnet restore

# install npm tools
RUN npm install -g 'clean-css-cli@latest'

# compile the css
WORKDIR /build/src/auth
COPY src/auth/package.json .
RUN npm install
COPY src/auth/scss/ scss/
RUN npm run sass:site

# copy site and build
WORKDIR /build
COPY . .
RUN bash -c ./docker-publish-auth-assets.sh
RUN dotnet build --no-restore -c Release -o out /build/src/auth/auth.csproj
RUN dotnet publish /build/src/auth/auth.csproj --no-restore -c Release -o out


# build runtime image
FROM base AS production
WORKDIR /app
COPY --from=build-env /build/src/auth/out .
ENTRYPOINT [ "dotnet", "maw_auth.dll" ]
