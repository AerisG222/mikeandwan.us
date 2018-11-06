source _vars.sh

echo 'Install dependencies for local projects? [y/n]'
read LOCAL_UPDATE

if [ "${LOCAL_UPDATE}" == "y" ]; then
    install_all_deps
fi
