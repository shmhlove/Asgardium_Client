
public enum eErrorCode
{
    // common
    Failed = 0,
    Succeed = 1,

    // network error (Sync Server)
    Net_Common_HTTP = 1000,
    Net_Common_InvalidParam = 1001,
    Net_Common_NotFoundCollection = 1002,
    Net_Common_DatabaseWrite = 1003,
    Net_Common_InvalidResponseData = 1004,
    Net_Common_JsonParse = 1005,
    Net_Common_CollectionRead = 1006,
    Net_Common_EmptyCollection = 1007,

    Net_Auth_NotFoundUser = 2001,
    Net_Auth_AlreadySignupUser = 2002,
    Net_Auth_InvalidPassword = 2003,

    // data error
    Data_NotImplementation = 1000001,
    Resources_NotExsitInTable = 1001002,
    Resources_LoadFailed = 1001003,
    Table_Not_AddClass = 1002001,
    Table_LoadFailed = 1002002,
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
    Update,
    Login,
    Lobby,
}

public enum eNationType
{
    Korea,
}

public enum eDataType
{
    None,
    Table,
    Resources,
}

public enum eTableLoadType
{
    None,
    Static,
    Server,
    Byte,
    Json,
    XML,
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

public enum eServiceMode
{
    None,
    Live,
    QA,
    Dev,
}

public enum eBundlePackType
{
    None,           // 아무것도 안함
    All,            // 전체 번들 리패킹
    Update,         // 변경된 리소스가 포함되는 번들만 패킹
}

public enum eDirection
{
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom
}