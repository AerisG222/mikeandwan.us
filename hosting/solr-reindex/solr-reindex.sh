#!/bin/bash
set -e

# delete all documents
curl -X POST -H 'Content-Type: application/json' --data-binary '{"delete":{"query":"*:*" }}' http://localhost:8983/solr/multimedia-categories/update/?commit=true

# kick off re-index
curl http://localhost:8983/solr/multimedia-categories/dataimport?command=full-import
