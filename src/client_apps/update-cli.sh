source _vars.sh

# 1: update global tooling
echo 'Update global ng cli? [y/n]'
read GLOBAL_UPDATE

if [ "${GLOBAL_UPDATE}" == "y" ]; then
    update_ngcli_global
fi

# 2: update all project tooling
echo 'Update local project cli? [y/n]'
read LOCAL_UPDATE

if [ "${LOCAL_UPDATE}" == "y" ]; then
    update_ngcli_all_projects
fi
