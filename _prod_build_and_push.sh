#!/bin/bash
IID_FILE=image.iid
TAG=$(date +%Y%m%d%H%M%S)

buildah bud -f Containerfile.api -t maw-api --iidfile "${IID_FILE}.api"
IID=$(cat "${IID_FILE}.api")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-api:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-api:latest

buildah bud -f Containerfile.auth -t maw-auth --iidfile "${IID_FILE}.auth"
IID=$(cat "${IID_FILE}.auth")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-auth:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-auth:latest

buildah bud -f Containerfile.www -t maw-www --iidfile "${IID_FILE}.www"
IID=$(cat "${IID_FILE}.www")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-www:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-www:latest

buildah bud -f Containerfile.cache -t maw-cache-sync --iidfile "${IID_FILE}.cache"
IID=$(cat "${IID_FILE}.cache")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-cache-sync:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-cache-sync:latest
