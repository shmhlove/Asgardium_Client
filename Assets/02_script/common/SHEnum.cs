﻿
public enum eErrorCode
{
    // common
    Failed = 0,
    Succeed = 1,

    // network error (Sync Server)
    Net_Common_HTTP = 1000,
    Net_Common_InvalidParam = 1001,
    Net_Common_DatabaseFind = 1002,
    Net_Common_DatabaseWrite = 1003,
    Net_Common_InvalidResponseData = 1004,
    Net_Common_JsonParse = 1005,

    Net_Auth_UserFind = 2001,
    Net_Auth_AlreadySignupUser = 2002,

    // data error
    Data_NotImplementation = 1000001,
    Resources_NotExsitInTable = 1001002,
    Resources_LoadFailed = 1001003,
    Table_Not_AddClass = 1002001,
    Table_Load_Fail = 1002002,
    Table_Not_Override = 1002004,
    Table_Not_ExsitFile = 1002005,
    Table_Error_Grammar = 1002006,

    // scene error
    Scene_Already_Loaded = 1010001,
}

public enum eSceneType
{
    None,
    Intro,
    Login,
}

public enum eNationType
{
    Korea,
}

public enum eDataType
{
    None,
    LocalTable,
    Resources,
}

public enum eTableType
{
    None,
    Static,
    Json,
    XML,
    Byte,
}

public enum eResourceType
{
    None,
    Prefab,
    Animation,
    Texture,
    Sound,
    Material,
    Text,
}

public enum eObjectPoolDestoryType
{
    Never,          // 제거 안함
    ChangeScene,    // 씬이 바뀔 때
}

public enum eServiceMode
{
    None,
    Live,           // Live
    Review,         // 리뷰제출용
    QA,             // QA용
    DevQA,          // 개발QA용
    Dev,            // 개발용
}

// 번들 패킹 타입
public enum eBundlePackType
{
    None,           // 아무것도 안함
    All,            // 전체 번들 리패킹
    Update,         // 변경된 리소스가 포함되는 번들만 패킹
}