source _vars.sh

# 1: update all project libs
echo 'Update angular project libs? [y/n]'
read LOCAL_UPDATE

if [ "${LOCAL_UPDATE}" == "y" ]; then
    update_ng_all_projects
fi
