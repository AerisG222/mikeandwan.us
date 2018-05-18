FROM nginx:1.13.9
WORKDIR /

RUN rm /etc/nginx/conf.d/default.conf

COPY src/nginx/caching_shared.conf /etc/nginx/caching_shared.conf
COPY src/nginx/mime.types          /etc/nginx/mime.types
COPY src/nginx/nginx.conf          /etc/nginx/nginx.conf
COPY src/nginx/proxy_shared.conf   /etc/nginx/proxy_shared.conf
COPY src/nginx/shared.conf         /etc/nginx/shared.conf
COPY src/nginx/tls_api.conf        /etc/nginx/tls_api.conf
COPY src/nginx/tls_auth.conf       /etc/nginx/tls_auth.conf
COPY src/nginx/tls_shared.conf     /etc/nginx/tls_shared.conf
COPY src/nginx/tls_www.conf        /etc/nginx/tls_www.conf

COPY src/nginx/conf.d/api.conf  /etc/nginx/conf.d/api.conf
COPY src/nginx/conf.d/auth.conf /etc/nginx/conf.d/auth.conf
COPY src/nginx/conf.d/www.conf  /etc/nginx/conf.d/www.conf
