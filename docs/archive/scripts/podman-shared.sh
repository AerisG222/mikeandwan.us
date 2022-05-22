create_pod() {
    local POD_NAME=${1}

    podman pod exists "${POD_NAME}"

    if [ $? -eq 1 ]; then
        echo "    - creating pod: ${POD_NAME}"
        podman pod create --name "${POD_NAME}" -p 8080:80 -p 8443:443
    fi
}

create_volume() {
    local VOL_NAME=${1}

    local VOL_INSPECTED=$(podman volume inspect "${VOL_NAME}" -f "{{.Name}}" 2> /dev/null)

    # volume inspect will match partial names - here we check that it was not found (first condition) or that the found name does not match (i.e. maw-postgres and maw-postgres-backup)
    # fortunately we do not have a case where there are more than 2 with the same prefix (fingers will remain crossed)
    if [ $? -ne 0 ] | [ "${VOL_INSPECTED}" != "${VOL_NAME}" ]; then
        echo "    - creating volume: ${VOL_NAME}"
        podman volume create "${VOL_NAME}"

        return 0
    fi

    return 1
}
