#!/bin/sh

progname=$(basename $0)

function usage ()
{
   cat <<EOF
Usage: $progname [Options] UNITY_PROJECT_DIR_PATH
- Options: -t TARGET_PLATFORM,      Input the text 'Android', 'IOS', 'StandaloneOSX', 'StandaloneWin'.
           -U UNITY_HOME_PATH,      Installed Unity Engine path.
           -b BUILD_NUMBER,
EOF
   exit 1
}

TARGET_PLATFORM=""
UNITY_HOME_PATH=""
BUILD_NUMBER="1"

while getopts ":t:hU:p:b:" opt; do
   case $opt in
   t )  TARGET_PLATFORM=$OPTARG ;;
   U )  UNITY_HOME_PATH=$OPTARG ;;
   b )  BUILD_NUMBER=$OPTARG ;;
   h )  usage ;;
   \?)  usage ;;
   esac
done
shift $(($OPTIND - 1))

if [ -z $TARGET_PLATFORM ]; then
    usage
fi

if [ -z $1 ]; then
    echo "* Invalid UNITY_PROJECT_DIR_PATH"
    echo
    usage
fi

if [ -z $UNITY_HOME_PATH ]; then
    usage
fi

if [ ! -f /usr/local/bin/greadlink ]; then
    brew install coreutils
fi

UNITY_HOME_PATH=$(/usr/local/bin/greadlink -f $UNITY_HOME_PATH)
PROJECT_PATH=$(/usr/local/bin/greadlink -f $1)
ARCHIVE_PATH="${PROJECT_PATH}/Build"

echo
echo "Target Platform: ${TARGET_PLATFORM}"
echo "Target Unity Project: ${PROJECT_PATH}"
echo "UNITY_HOME_PATH: ${UNITY_HOME_PATH}"
echo "ARCHIVE_PATH: ${ARCHIVE_PATH}"
echo

pushd "$PROJECT_PATH"

if [ -d $ARCHIVE_PATH ]; then
    rm -rf ${ARCHIVE_PATH}
fi
mkdir $ARCHIVE_PATH


if [ ${TARGET_PLATFORM} == "IOS" ]; then
    # Unity 에서 Xcode Project 생성.
    echo "Building... Xcode project"
    eval "\"${UNITY_HOME_PATH}/Unity2018.app/Contents/MacOS/Unity\"" \
    -quit \
    -batchmode \
    -nographics \
    -buildTarget iOS \
    -executeMethod "AutoBuilder.PerformiOSBuildDebugReplace" \
    -logFile \
    -projectPath ${PROJECT_PATH} | tee -a "${ARCHIVE_PATH}/IOS.build.log"

    is_build_failed=false
    tmp_cmd=`cat ${ARCHIVE_PATH}/${TARGET_PLATFORM}.build.log | grep "Exiting batchmode successfully now!" | wc -l`
    if [[ "$tmp_cmd" -eq 0 ]]; then
    	is_build_failed=true
    fi

    if $is_build_failed; then
    cat "${ARCHIVE_PATH}/${TARGET_PLATFORM}.build.log" | grep "Scripts have compiler errors."
    exit 1
    fi

    #Xcode Project 에 fastlane 파일 복사
    cp -avR "${PROJECT_PATH}/fastlane" "${PROJECT_PATH}/Build/iOS"

    pushd "${PROJECT_PATH}/Build/iOS"
    APP_VERSION=$(agvtool what-marketing-version -terse1)
    fastlane ios build target:Unity-iPhone build_number:${BUILD_NUMBER}

    if [ ! -e "${ARCHIVE_PATH}/iOS/ipa-dir/test.ipa" ]; then
        echo "Build FAILED: Not exist the file: ${ARCHIVE_PATH}/iOS/ipa-dir/test.ipa"
        exit 1
    fi

    cp "${ARCHIVE_PATH}/iOS/ipa-dir/test.ipa" "${ARCHIVE_PATH}/NcPlatformSdkForUnityTest_QA_${APP_VERSION}.${BUILD_NUMBER}.ipa"
    popd

elif [ ${TARGET_PLATFORM} == "Android" ]; then
    echo "Builing... APK"
    eval "\"${UNITY_HOME_PATH}/Unity2018.app/Contents/MacOS/Unity\"" \
    -quit \
    -batchmode \
    -nographics \
    -executeMethod "SHBuildScript.KOR_AndroidAppBuild" \
    -logFile \
    -projectPath ${PROJECT_PATH} | tee -a "${ARCHIVE_PATH}/Android.build.log"

    is_build_failed=false
    tmp_cmd=`cat ${ARCHIVE_PATH}/${TARGET_PLATFORM}.build.log | grep "Exiting batchmode successfully now!" | wc -l`
    if [[ "$tmp_cmd" -eq 0 ]]; then
    	is_build_failed=true
    fi

    if $is_build_failed; then
    cat "${ARCHIVE_PATH}/${TARGET_PLATFORM}.build.log" | grep "Scripts have compiler errors."
    exit 1
    fi

elif [ ${TARGET_PLATFORM} == "StandaloneOSX" ]; then
    echo "Builing... StandaloneOSX"
    eval "\"${UNITY_HOME_PATH}/Unity2018.app/Contents/MacOS/Unity\"" \
    -quit \
    -batchmode \
    -nographics \
    -buildTarget standalone \
    -executeMethod "AutoBuilder.PerformStandaloneOSXBuildDebug" \
    -logFile \
    -projectPath ${PROJECT_PATH} | tee -a "${ARCHIVE_PATH}/StandaloneOSX.build.log"

    is_build_failed=false
    tmp_cmd=`cat ${ARCHIVE_PATH}/${TARGET_PLATFORM}.build.log | grep "Exiting batchmode successfully now!" | wc -l`
    if [[ "$tmp_cmd" -eq 0 ]]; then
    	is_build_failed=true
    fi

    if $is_build_failed; then
    cat "${ARCHIVE_PATH}/${TARGET_PLATFORM}.build.log" | grep "Scripts have compiler errors."
    exit 1
    fi

elif [ ${TARGET_PLATFORM} == "StandaloneWin" ]; then
    echo "Builing... StandaloneWin"
    eval "\"${UNITY_HOME_PATH}/Unity2018.app/Contents/MacOS/Unity\"" \
    -quit \
    -batchmode \
    -nographics \
    -buildTarget standalone \
    -executeMethod "AutoBuilder.PerformStandaloneWindows64BuildDebug" \
    -logFile \
    -projectPath ${PROJECT_PATH} | tee -a "${ARCHIVE_PATH}/StandaloneWin.build.log"

    is_build_failed=false
    tmp_cmd=`cat ${ARCHIVE_PATH}/${TARGET_PLATFORM}.build.log | grep "Exiting batchmode successfully now!" | wc -l`
    if [[ "$tmp_cmd" -eq 0 ]]; then
    	is_build_failed=true
    fi

    if $is_build_failed; then
    cat "${ARCHIVE_PATH}/${TARGET_PLATFORM}.build.log" | grep "Scripts have compiler errors."
    exit 1
    fi

    pushd "$ARCHIVE_PATH/Windows"
    zip -r -X "$ARCHIVE_PATH/$TARGET_PLATFORM.zip" "./"
    popd
else
    echo "Input a vaild -t option value."
    echo
    usage
fi

# set +e
# echo "BUILD SUCCEED!!"

popd
