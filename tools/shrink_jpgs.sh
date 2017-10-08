IMAGE_PATH=$1

optimize_jpg() {
    local FILE=$1

    jpegoptim --strip-all --all-progressive -m72 "${FILE}"
    jpegtran -optimize -copy none -outfile "${FILE}" "${FILE}"
}

get_value() {
    local prompt=$1
    local secure=$2
    local val=

    while [ "${val}" = "" ]
    do
        if [ "${secure}" = "y" ]; then
            read -e -r -s -p "${prompt}" val
        else
            read -e -r -p "${prompt}" val
        fi
    done

    echo "${val}"
}

if [ "${IMAGE_PATH}" = "" ]; then
    IMAGE_PATH=$(get_value 'Path to directory / file to process: ' 'n')

    # cleanup trailing slash if needed
    IMAGE_PATH="${IMAGE_PATH%/}"
fi

if [ -d "${IMAGE_PATH}" ]; then
    FILES=$(find "${IMAGE_PATH}" -type f -name "*.jpg" -not -path "*/prt/*" -not -path "*/src/*")

    for F in ${FILES}
    do
        optimize_jpg "$F"
    done
else
    optimize_jpg "${IMAGE_PATH}"
fi
