#!/bin/bash

pushd hosting

pushd certs
buildah bud -f Containerfile.dev -t maw-certs-dev
popd

pushd gateway
buildah bud -f Containerfile.dev -t maw-gateway-dev
popd

pushd postgres-maintenance
buildah bud -f Containerfile -t maw-postgres-maintenance-dev
popd

pushd solr
buildah bud -f Containerfile -t maw-solr-dev
popd

pushd solr-reindex
buildah bud -f Containerfile -t maw-solr-reindex-dev
popd

popd

# volume mount is a workaround for a current COPY perf issue
buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.api -t maw-api-dev
buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.auth -t maw-auth-dev
buildah bud -v "$(pwd)":/tmp/context:ro,Z -f Containerfile.www -t maw-www-dev
