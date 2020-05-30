#!/bin/bash
IID_FILE=image.iid
TAG=$(date +%Y%m%d%H%M%S)

pushd hosting

pushd certs
buildah bud -f Containerfile.prod -t maw-certs --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-certs:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-certs:latest
popd

pushd gateway
buildah bud -f Containerfile.prod -t maw-gateway --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-gateway:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-gateway:latest
popd

pushd postgres-maintenance
buildah bud -f Containerfile -t maw-postgres-maintenance --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-postgres-maintenance:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-postgres-maintenance:latest
popd

pushd solr
buildah bud -f Containerfile -t maw-solr --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-solr:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-solr:latest
popd

pushd solr-reindex
buildah bud -f Containerfile -t maw-solr-reindex --iidfile "${IID_FILE}"
IID=$(cat "${IID_FILE}")
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-solr-reindex:"${TAG}"
buildah push --creds "${DH_USER}:${DH_PASS}" "${IID}" docker://aerisg222/maw-solr-reindex:latest
popd

popd

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
