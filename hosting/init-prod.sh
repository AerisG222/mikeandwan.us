#!/bin/bash

# to initialize prod - first follow the init-dev steps,
# then run these additional items to configure production cert acquisition
podman volume create maw-certbot-validation
podman volume create maw-certbot-certs
