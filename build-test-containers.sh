#!/bin/bash

buildah bud -f Containerfile.api   -t maw-api-test
buildah bud -f Containerfile.auth  -t maw-auth-test
buildah bud -f Containerfile.www   -t maw-www-test
buildah bud -f Containerfile.cache -t maw-cache-sync-test
