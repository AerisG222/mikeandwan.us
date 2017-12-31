echo '***********************************************************************************************'
echo 'This script should run in an empty directory that is only used for holding one test certificate'
echo 'Make sure you note the password to use for the certificate in Program.cs'
echo '***********************************************************************************************'
read -rsp $'Press enter to continue, or CTL-C to exit...\n'

#1: generate a self signed cert:
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365

#2: combine into one pfx for kestrel:
openssl pkcs12 -inkey key.pem -in cert.pem -export -out testcert.pfx

#3: cleanup
rm *.pem
