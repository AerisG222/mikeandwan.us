# https://github.com/dotnet/dotnet-docker-samples/blob/master/aspnetapp/Dockerfile
# https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files/
FROM microsoft/aspnetcore AS base
RUN apt-get update && \
    apt-get install -y imagemagick && \
    rm -rf /var/lib/apt/lists/*
RUN ln -s /usr/lib/x86_64-linux-gnu/libMagickWand-6.Q16.so.3 /usr/lib/x86_64-linux-gnu/libMagickWand-6.Q16.so
WORKDIR /app
EXPOSE 5021


# build env is ordered to try and maximize caching for faster builds
FROM microsoft/aspnetcore-build AS build-env
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

# install the ng cli
RUN npm install -g '@angular/cli@latest'

# copy all the app package.jsons and shell scripts, then install the libs
COPY src/client_apps/bandwidth/package.json src/client_apps/bandwidth/package.json
COPY src/client_apps/binary_clock/package.json      src/client_apps/binary_clock/package.json
COPY src/client_apps/byte_counter/package.json      src/client_apps/byte_counter/package.json
COPY src/client_apps/filesize/package.json          src/client_apps/filesize/package.json
COPY src/client_apps/googlemaps/package.json        src/client_apps/googlemaps/package.json
COPY src/client_apps/learning/package.json          src/client_apps/learning/package.json
COPY src/client_apps/memory/package.json            src/client_apps/memory/package.json
COPY src/client_apps/money_spin/package.json        src/client_apps/money_spin/package.json
COPY src/client_apps/photo_stats/package.json       src/client_apps/photo_stats/package.json
COPY src/client_apps/photos/package.json            src/client_apps/photos/package.json
COPY src/client_apps/photos3d/package.json          src/client_apps/photos3d/package.json
COPY src/client_apps/time/package.json              src/client_apps/time/package.json
COPY src/client_apps/videos/package.json            src/client_apps/videos/package.json
COPY src/client_apps/webgl_cube/package.json        src/client_apps/webgl_cube/package.json
#COPY src/client_apps/webgl_shader/package.json     src/client_apps/webgl_shader/package.json
COPY src/client_apps/webgl_text/package.json        src/client_apps/webgl_text/package.json
COPY src/client_apps/weekend_countdown/package.json src/client_apps/weekend_countdown/package.json
COPY src/client_apps/*.sh                           src/client_apps/
WORKDIR /build/src/client_apps
RUN bash -c ./docker-install-libs.sh

# now copy all the js code and build everything
COPY src/client_apps /build/src/client_apps
RUN bash -c ./docker-build.sh

# compile the css
WORKDIR /build/src/www
COPY src/www/package.json .
RUN npm install
COPY src/www/scss/ scss/
RUN npm run sass:site && \
    npm run sass:games

# copy site and build
WORKDIR /build/
COPY . .
RUN ./docker-publish-js-apps.sh
RUN dotnet build --no-restore -c Release -o out /build/src/www/www.csproj
RUN dotnet publish /build/src/www/www.csproj --no-restore -c Release -o out


# build runtime image
FROM base AS production
WORKDIR /app
COPY --from=build-env /build/src/www/out .
ENTRYPOINT [ "dotnet", "maw_www.dll" ]
