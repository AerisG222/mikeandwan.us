#!/bin/bash

pushd certs
buildah bud -f Containerfile.dev -t maw-certs-dev
popd

pushd gateway
buildah bud -f Containerfile.dev -t maw-gateway-dev
popd

pushd solr
buildah bud -f Containerfile -t maw-solr-dev
popd
