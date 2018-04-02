# https://gist.github.com/kyledrake/d7457a46a03d7408da31
# https://andrewlock.net/creating-and-trusting-a-self-signed-certificate-on-linux-for-use-in-kestrel-and-asp-net-core/
# https://www.humankode.com/asp-net-core/develop-locally-with-https-self-signed-certificates-and-asp-net-core
# https://blog.zencoffee.org/2013/04/creating-and-signing-an-ssl-cert-with-alternative-names/

IS_PROD="$1"
DATE=`date -Idate`
SCRIPT=`realpath "${BASH_SOURCE}"`
SCRIPT_DIR="${SCRIPT%/*}"
CONF_PATH="${SCRIPT_DIR}/cert.conf"
EXT_PATH="${SCRIPT_DIR}/cert.ext"

if [ "${IS_PROD}" == 'y' ]
then
    if [ $EUID -ne 0 ]
    then
        echo 'This script must run with root privleges when configuring production.  Exiting.'
        exit 1
    fi

    echo 'Environment: PRODUCTION'
    echo ''

    CERT_ROOT="/etc/maw_certs"
else
    echo 'Environment: DEVELOPMENT'
    echo ''

    CERT_ROOT="${SCRIPT%/*/*}/maw_certs"
fi

CA_DIR="${CERT_ROOT}/ca"
CA_KEY="${CA_DIR}/ca_${DATE}.key"
CA_CRT="${CA_DIR}/ca_${DATE}.crt"
# NSSDB="sql:${HOME}/.pki/nssdb"

PROJECTS=(
    "api"
    "auth"
    "www"
)

echo "This script will create a CA and certs for auth (HTTPS + Signing), api, and www endpoints."
echo "These are to be used by Kestrel and IdentityServer, whereas LetsEncrypt is used on the public"
echo "facing endpoints (in production)."
echo ''
echo "Certs will be created at the following root directory: ${CERT_ROOT}."
echo ''
read -rsp $'Press enter to continue, or CTL-C to exit...\n'

gen_cert() {
    local project=$1
    local certdir="${CERT_ROOT}/${project}"
    local certkey="${certdir}/${project}_${DATE}.key"
    local certreq="${certdir}/${project}_${DATE}.csr"
    local certcrt="${certdir}/${project}_${DATE}.crt"
    local certpfx="${certdir}/${project}_${DATE}.pfx"
    local activepfx="${certdir}/${project}.pfx"

    echo ''
    echo ''
    echo '*********************************************************'
    echo " creating certificate for -[${project}]-"
    echo ' when asked for the Common Name, USE localhost'
    echo '*********************************************************'

    if [ ! -d "${certdir}" ]
    then
        mkdir -p "${certdir}"
    fi

    # Generate the domain key:
    openssl genrsa -out "${certkey}" 2048

    # Generate the certificate signing request
    openssl req -sha256 -days 400 -new -nodes -config "${CONF_PATH}" -key "${certkey}" -out "${certreq}"

    # Sign the request with your root key
    openssl x509 -sha256 -days 400 -req -extfile "${EXT_PATH}" -in "${certreq}" -CA "${CA_CRT}" -CAkey "${CA_KEY}" -CAcreateserial -out "${certcrt}"

    # Check your homework:
    openssl verify -CAfile "${CA_CRT}" "${certcrt}"

    # Make pfx for kestrel
    openssl pkcs12 -export -out "${certpfx}" -inkey "${certkey}" -in "${certcrt}"

    # create symlink to new cert in order to activate
    if [ -e "${activepfx}" ]
    then
        rm "${activepfx}"
    fi

    ln -s "${certpfx}" "${activepfx}"

    # Trust the certificate for SSL
    # pk12util -d "${NSSDB}" -i "${certpfx}"

    # Trust a self-signed server certificate
    # certutil -d "${NSSDB}" -A -t "P,," -n "maw:${project}" -i "${certcrt}"
}

if [ ! -d "${CA_DIR}" ]; then
    mkdir -p "${CA_DIR}"
fi

# Generate the root (GIVE IT A PASSWORD IF YOU'RE NOT AUTOMATING SIGNING!):
echo ''
echo ''
echo '*********************************************************'
echo ' creating certificate authority'
echo ' when asked for the Common Name, do NOT use localhost'
echo '*********************************************************'
openssl genrsa -aes256 -out "${CA_KEY}" 2048
openssl req -new -x509 -days 400 -key "${CA_KEY}" -sha256 -extensions v3_ca -out "${CA_CRT}"

# Trust our ca
# certutil -d "${NSSDB}" -A -t "c,," -n "maw_ca" -i "${CA_CRT}"

if [ $EUID -ne 0 ]
then
    echo 'You will be prompted for your password to publish the new CA'
    echo 'to make it available machine-wide via sudo...'
fi

sudo cp "${CA_CRT}" /usr/share/pki/ca-trust-source/anchors/
sudo update-ca-trust

for I in "${PROJECTS[@]}"
do
    gen_cert "${I}"
done

# Add the trusted certificate to the system:
# sudo cp neocities.ca.crt /usr/local/share/ca-certificates/
# sudo update-ca-certificates
