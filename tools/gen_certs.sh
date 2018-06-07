# https://gist.github.com/kyledrake/d7457a46a03d7408da31
# https://andrewlock.net/creating-and-trusting-a-self-signed-certificate-on-linux-for-use-in-kestrel-and-asp-net-core/
# https://www.humankode.com/asp-net-core/develop-locally-with-https-self-signed-certificates-and-asp-net-core
# https://blog.zencoffee.org/2013/04/creating-and-signing-an-ssl-cert-with-alternative-names/

IS_PROD="$1"
DATE=`date -Idate`
CA_TTL_DAYS=1825  # 5yrs
CA_REGEN_THRESHOLD=180  # > CERT_TTL_DAYS to ensure the CA always lives longer than issued certs
CERT_TTL_DAYS=120  # certs should rotate every 90 days
CERT_REGEN_THRESHOLD=30
CA_SUBJ="/C=US/ST=Massachusetts/L=Boston/O=mikeandwan.us/OU=IT/CN=maw_ca"
CERT_SUBJ="/C=US/ST=Massachusetts/L=Boston/O=mikeandwan.us/OU=IT/CN="
SCRIPT=`realpath "${BASH_SOURCE}"`
SCRIPT_DIR="${SCRIPT%/*}"
RESTART_SITES="n"

if [ "${IS_PROD}" == 'y' ]
then
    echo 'Environment: PRODUCTION'
    echo ''

    CERT_ROOT="/home/svc_www_maw/maw_certs"
else
    echo 'Environment: DEVELOPMENT'
    echo ''

    CERT_ROOT="${SCRIPT%/*/*}/maw_certs"
fi

CA_DIR="${CERT_ROOT}/ca"
CA_KEY="${CA_DIR}/ca.key"
CA_KEY_PWD="${CA_KEY}.pwd"
CA_CRT="${CA_DIR}/ca.crt"
CA_CRT_PWD="${CA_CRT}.pwd"
# NSSDB="sql:${HOME}/.pki/nssdb"

PROJECTS=(
    "api"
    "auth"
    "signing"
    "www"
)

if [ "${IS_PROD}" != 'y' ]
then
    echo "This script will create a CA and certs for auth (HTTPS + Signing), api, and www endpoints."
    echo "These are to be used by Kestrel and IdentityServer, whereas LetsEncrypt is used on the public"
    echo "facing endpoints on nginx (in production)."
    echo ''
    echo "Certs will be created at the following root directory: ${CERT_ROOT}."
    echo ''
    read -rsp $'Press enter to continue, or CTL-C to exit...\n'
fi

will_expire_soon() {
    local cert="$1"
    local threshold="$2"
    local secs_in_day=86400
    local threshold_secs=$(expr $threshold \* $secs_in_day)

    if [ -e "${cert}" ]
    then
        echo `openssl x509 -in "${cert}" -checkend "${threshold_secs}" |grep -c "will expire"`
    else
        echo "1"
    fi
}

gen_pwd() {
    openssl rand -base64 32
}

gen_cert() {
    local project="$1"
    local certdir="${CERT_ROOT}/${project}"
    local certkey="${certdir}/${project}_${DATE}.key"
    local certreq="${certdir}/${project}_${DATE}.csr"
    local certcrt="${certdir}/${project}_${DATE}.crt"
    local certpfx="${certdir}/${project}_${DATE}.pfx"
    local certkeypwd="${certkey}.pwd"
    local certpfxpwd="${certpfx}.pwd"
    local activecrt="${certdir}/${project}.crt" # used by this script
    local activepfx="${certdir}/${project}.pfx" # used by server
    local activekey="${certdir}/${project}.key"
    local activekeypwd="${activekey}.pwd"
    local activepfxpwd="${activepfx}.pwd"

    if [ ! -d "${certdir}" ]
    then
        mkdir -p "${certdir}"
    fi

    local cert_expiring=$(will_expire_soon "${activecrt}" "${CERT_REGEN_THRESHOLD}")

    if [ "${cert_expiring}" = "0" ]
    then
        echo ''
        echo "* cert for ${project} is not expiring soon, it will not be generated."
        echo ''
    else
        echo ''
        echo '*********************************************************'
        echo "* creating certificate for -[${project}]-"
        echo '*********************************************************'
        echo ''

        RESTART_SITES="y"

        # create password for the keyfile
        PASSWORD=$(gen_pwd)
        echo "${PASSWORD}" > "${certkeypwd}"

        # create password for the cert
        PASSWORD=$(gen_pwd)
        echo "${PASSWORD}" > "${certpfxpwd}"

        PASSWORD=

        # Generate the domain key:
        openssl genrsa -passout "file:${certkeypwd}" -out "${certkey}" 2048

        # Generate the certificate signing request
        subj="${CERT_SUBJ}${project}dev.mikeandwan.us"
        openssl req -sha256 -days "${CERT_TTL_DAYS}" -subj "${subj}" -new -nodes -config "${SCRIPT_DIR}/cert_cfg/${project}.conf" -key "${certkey}" -passin "file:${certkeypwd}" -out "${certreq}"

        # Sign the request with your root key
        openssl x509 -sha256 -days "${CERT_TTL_DAYS}" -req -extfile "${SCRIPT_DIR}/cert_cfg/${project}.ext" -in "${certreq}" -CA "${CA_CRT}" -CAkey "${CA_KEY}" -passin "file:${CA_KEY_PWD}" -CAcreateserial -out "${certcrt}"

        # Check your homework:
        openssl verify -CAfile "${CA_CRT}" "${certcrt}"

        # Make pfx for kestrel
        openssl pkcs12 -export -out "${certpfx}" -inkey "${certkey}" -in "${certcrt}" -passin "file:${certkeypwd}" -passout "file:${certpfxpwd}"

        # create symlink to new cert to make it available to services
        if [ -e "${activepfx}" ]
        then
            rm "${activepfx}"
            rm "${activecrt}"
            rm "${activekey}"
            rm "${activekeypwd}"
            rm "${activepfxpwd}"
        fi

        ln -s "${certkey}" "${activekey}"
        ln -s "${certkeypwd}" "${activekeypwd}"
        ln -s "${certcrt}" "${activecrt}"
        ln -s "${certpfx}" "${activepfx}"
        ln -s "${certpfxpwd}" "${activepfxpwd}"

        # Trust the certificate for SSL
        # pk12util -d "${NSSDB}" -i "${certpfx}"

        # Trust a self-signed server certificate
        # certutil -d "${NSSDB}" -A -t "P,," -n "maw:${project}" -i "${certcrt}"
    fi
}

# *************************************
# ROOT CA
# *************************************
if [ ! -d "${CA_DIR}" ]; then
    mkdir -p "${CA_DIR}"
fi

CA_CERT_EXPIRING=$(will_expire_soon "${CA_CRT}" "${CA_REGEN_THRESHOLD}")

if [ "${CA_CERT_EXPIRING}" = "0" ]
then
    echo '* CA cert is not expiring soon, it will not be generated.'
    echo ''
else
    RESTART_SITES="y"

    CA_KEY_NEW="${CA_DIR}/ca_${DATE}.key"
    CA_KEY_PWD_NEW="${CA_KEY_NEW}.pwd"
    CA_CRT_NEW="${CA_DIR}/ca_${DATE}.crt"
    CA_CRT_PWD_NEW="${CA_CRT_NEW}.pwd"

    echo '*********************************************************'
    echo '* creating certificate authority'
    echo '*********************************************************'
    echo ''

    # create password for the keyfile
    PASSWORD=$(gen_pwd)
    echo "${PASSWORD}" > "${CA_KEY_PWD_NEW}"

    # create password for the cert
    PASSWORD=$(gen_pwd)
    echo "${PASSWORD}" > "${CA_CRT_PWD_NEW}"

    PASSWORD=

    # create CA key
    openssl genrsa -aes256 -passout "file:${CA_KEY_PWD_NEW}" -out "${CA_KEY_NEW}" 2048

    # create CA cert
    openssl req -new -x509 -days "${CA_TTL_DAYS}" -key "${CA_KEY_NEW}" -subj "${CA_SUBJ}" -passin "file:${CA_KEY_PWD_NEW}" -passout "file:${CA_CRT_PWD_NEW}" -sha256 -extensions v3_ca -out "${CA_CRT_NEW}"

    # update symlink to point at current
    if [ -e "${CA_CRT}" ]
    then
        rm "${CA_KEY}"
        rm "${CA_CRT}"
        rm "${CA_KEY_PWD}"
        rm "${CA_CRT_PWD}"
    fi

    ln -s "${CA_KEY_NEW}" "${CA_KEY}"
    ln -s "${CA_KEY_PWD_NEW}" "${CA_KEY_PWD}"
    ln -s "${CA_CRT_NEW}" "${CA_CRT}"
    ln -s "${CA_CRT_PWD_NEW}" "${CA_CRT_PWD}"

    # Trust our ca
    # certutil -d "${NSSDB}" -A -t "c,," -n "maw_ca" -i "${CA_CRT}"

    if [ $EUID -ne 0 ]
    then
        echo 'You will be prompted for your password to publish the new CA'
        echo 'to make it available machine-wide via sudo...'
        echo ''
    fi

    sudo cp "${CA_CRT_NEW}" /usr/share/pki/ca-trust-source/anchors/
    sudo update-ca-trust
fi

# *************************************
# PROJECT CERTS
# *************************************
for I in "${PROJECTS[@]}"
do
    gen_cert "${I}"
done

# Add the trusted certificate to the system:
# sudo cp neocities.ca.crt /usr/local/share/ca-certificates/
# sudo update-ca-certificates

if [ "${RESTART_SITES}" == 'y' ] && [ "${IS_PROD}" != 'y' ]
then
    chown -R svc_www_maw:svc_www_maw "${CERT_ROOT}"

    echo 'Restarting web services...'

    systemctl stop maw_api.service
    systemctl stop maw_auth.service
    systemctl stop maw_www.service

    systemctl start maw_api.service
    systemctl start maw_auth.service
    systemctl start maw_www.service
fi
