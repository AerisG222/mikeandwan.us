FROM alpine

RUN apk --no-cache add \
    bash \
    curl

WORKDIR /

COPY solr-reindex.sh /usr/local/bin

CMD [ "solr-reindex.sh" ]
