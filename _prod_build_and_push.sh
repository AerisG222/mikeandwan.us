#!/bin/bash
IID_FILE=image.iid
TAG=$(date +%Y%m%d%H%M%S)

# volume mount is a workaround for a current COPY perf issue
buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.api -t maw-api --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-api:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-api:latest

buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.auth -t maw-auth --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-auth:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-auth:latest

buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.www -t maw-www --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-www:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-www:latest

buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.cache -t maw-cache-sync --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-cache-sync:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-cache-sync:latest
