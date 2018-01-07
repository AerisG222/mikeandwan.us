# https://andrewlock.net/creating-and-trusting-a-self-signed-certificate-on-linux-for-use-in-kestrel-and-asp-net-core/
# https://www.humankode.com/asp-net-core/develop-locally-with-https-self-signed-certificates-and-asp-net-core

echo '***********************************************************************************************'
echo 'This script should run in an empty directory that is only used for holding one test certificate'
echo 'Make sure you note the password to use for the certificate in Program.cs'
echo '***********************************************************************************************'
read -rsp $'Press enter to continue, or CTL-C to exit...\n'
read -rp $'Enter nickname for this cert (auth, api, www...): ' CERT_NICKNAME

SCRIPT_PATH=`dirname "$0"`
CONF_PATH="${SCRIPT_PATH}/make_test_cert.conf"
KEY="${CERT_NICKNAME}.key"
CERT="${CERT_NICKNAME}.crt"
PFX="${CERT_NICKNAME}.pfx"

#1: generate key and self signed cert
openssl req \
        -config "${CONF_PATH}" \
        -new \
        -x509 \
        -sha256 \
        -newkey rsa:2048 \
        -nodes \
        -days 3650 \
        -keyout "${KEY}" \
        -out "${CERT}"

#2: combine for kestrel
openssl pkcs12 \
        -export \
        -out "${PFX}" \
        -inkey "${KEY}" \
        -in "${CERT}"

#3: trust cert
# Trust the certificate for SSL
pk12util -d sql:$HOME/.pki/nssdb -i "${PFX}"

# Trust a self-signed server certificate
certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n "${CERT_NICKNAME}" -i "${CERT}"
