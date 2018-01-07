# https://gist.github.com/kyledrake/d7457a46a03d7408da31
# https://andrewlock.net/creating-and-trusting-a-self-signed-certificate-on-linux-for-use-in-kestrel-and-asp-net-core/
# https://www.humankode.com/asp-net-core/develop-locally-with-https-self-signed-certificates-and-asp-net-core
# https://blog.zencoffee.org/2013/04/creating-and-signing-an-ssl-cert-with-alternative-names/

CLEAN=$1
SCRIPT=`realpath "${BASH_SOURCE}"`
ROOT="${SCRIPT%/*/*}"
CONF_PATH="${ROOT}/tools/cert.conf"
EXT_PATH="${ROOT}/tools/cert.ext"
CERTDIR="certs"
CADIR="${ROOT}/${CERTDIR}"
CA_KEY="${CADIR}/ca.key"
CA_CRT="${CADIR}/ca.crt"
NSSDB="sql:${HOME}/.pki/nssdb"

PROJECTS=(
    "api"
    "auth"
    "www"
)

if [ "${CLEAN}" == 'y' ]; then
    echo "This script will remove previously generated test CA and endpoint test certs"
else
    echo "This script will create a CA and certs for auth, api, and www dev endpoints"
fi
echo ''
read -rsp $'Press enter to continue, or CTL-C to exit...\n'

get_cert_dir() {
    local project=$1

    echo "${ROOT}/src/${project}/${CERTDIR}"
}

gen_cert() {
    local project=$1
    local certdir="$(get_cert_dir ${project})"
    local certkey="${certdir}/${project}.key"
    local certreq="${certdir}/${project}.csr"
    local certcrt="${certdir}/${project}.crt"
    local certpfx="${certdir}/${project}.pfx"

    echo ''
    echo ''
    echo '*********************************************************'
    echo " creating certificate for -[${project}]-"
    echo ' when asked for the Common Name, USE localhost'
    echo '*********************************************************'

    if [ ! -d "${certdir}" ]; then
        mkdir "${certdir}"
    fi

    # Generate the domain key:
    openssl genrsa -out "${certkey}" 2048

    # Generate the certificate signing request
    openssl req -sha256 -days 3650 -new -nodes -config "${CONF_PATH}" -key "${certkey}" -out "${certreq}"

    # Sign the request with your root key
    openssl x509 -sha256 -days 3650 -req -extfile "${EXT_PATH}" -in "${certreq}" -CA "${CA_CRT}" -CAkey "${CA_KEY}" -CAcreateserial -out "${certcrt}"

    # Check your homework:
    openssl verify -CAfile "${CA_CRT}" "${certcrt}"

    # Make pfx for kestrel
    openssl pkcs12 -export -out "${certpfx}" -inkey "${certkey}" -in "${certcrt}"

    # Trust the certificate for SSL
    #pk12util -d "${NSSDB}" -i "${certpfx}"

    # Trust a self-signed server certificate
    #certutil -d "${NSSDB}" -A -t "P,," -n "maw:${project}" -i "${certcrt}"
}

clean_cert() {
    local project=$1
    local certdir="$(get_cert_dir ${project})"

    if [ -d "${certdir}" ]; then
        rm -rf "${certdir}"
    fi

    # Untrust the certificate for SSL
    #certutil -d "${NSSDB}" -D -n "NSS Certificate DB:maw:${project}"

    # Untrust a self-signed server certificate
    #certutil -d "${NSSDB}" -D -n "maw:${project}"
}

if [ "${CLEAN}" == 'y' ]; then
    # Untrust our ca
    #certutil -d "${NSSDB}" -D -n "maw_ca"

    if [ -d "${CADIR}" ]; then
        rm -rf "${CADIR}"
    fi

    for I in "${PROJECTS[@]}"
    do
        clean_cert "${I}"
    done
else
    if [ ! -d "${CADIR}" ]; then
        mkdir "${CADIR}"
    fi

    # Generate the root (GIVE IT A PASSWORD IF YOU'RE NOT AUTOMATING SIGNING!):
    echo ''
    echo ''
    echo '*********************************************************'
    echo ' creating certificate authority'
    echo ' when asked for the Common Name, do NOT use localhost'
    echo '*********************************************************'
    openssl genrsa -aes256 -out "${CA_KEY}" 2048
    openssl req -new -x509 -days 3650 -key "${CA_KEY}" -sha256 -extensions v3_ca -out "${CA_CRT}"

    # Trust our ca
    #certutil -d "${NSSDB}" -A -t "c,," -n "maw_ca" -i "${CA_CRT}"

    echo 'sudo + publish CA to make it available machine-wide...'
    sudo cp "${CA_CRT}" /usr/share/pki/ca-trust-source/anchors/
    sudo update-ca-trust

    for I in "${PROJECTS[@]}"
    do
        gen_cert "${I}"
    done

    # Add the trusted certificate to the system:
    # sudo cp neocities.ca.crt /usr/local/share/ca-certificates/
    # sudo update-ca-certificates
fi
