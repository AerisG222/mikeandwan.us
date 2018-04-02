source _vars.sh

echo 'Install dependencies for all projects? [y/n]'
read DOWORK

if [ "${DOWORK}" == "y" ]; then
    install_all_libs
fi
