#!/bin/bash

# volume mount is a workaround for a current COPY perf issue
buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.api -t maw-api-dev
buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.auth -t maw-auth-dev
buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.www -t maw-www-dev
