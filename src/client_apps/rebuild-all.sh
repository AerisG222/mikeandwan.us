source _vars.sh

DOWORK=$1
PROD=$2

if [ "${DOWORK}" == '' ]
then
    echo 'Execute build for all projects? [y/n]'
    read DOWORK
fi

if [ "${DOWORK}" == 'y' ]
then
    if [ "${PROD}" == 'y' ]
    then
        build_all_apps prod_build 'y'
    else
        build_all_apps dev_build 'n'
    fi
fi
