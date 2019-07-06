
public enum eErrorCode
{
    // common
    Failed = 0,
    Succeed = 1,

    // network error
    Net_Common_HTTP = 1000,

    // Server error (Sync ServerErrorCode)
    Server_Auth_AlreadySignupUser = 2001,
    Server_Auth_NoSignupUser = 2002,
    Server_Auth_NoMatchPassword = 2003,
    Server_Mining_ZeroSupplyQuantity = 4001,
    Server_Mining_NotEnoughMiningPower = 4002,

    // net common error
    Net_InvalidResponseData = 100001,
    Net_JsonParse = 100002,

    // socket error
    Net_Socket_Disconnect = 101001,
    Net_Socket_Connect_Error = 101002,
    Net_Socket_Connect_Timeout = 101003,
    Net_Socket_Aready_Connect = 101004,

    // data error
    Data_NotImplementation = 102001,
    Resources_NotExsitInTable = 102002,
    Resources_LoadFailed = 102003,
    Table_Not_AddedClass = 102004,
    Table_LoadFailed = 102005,
    Table_Not_OverrideFunc = 102006,
    Table_Not_ExsitFile = 102007,
    Table_Error_Grammar = 102008,

    // scene error
    Scene_Already_Loaded = 103001,

    // Auth
    Auth_InvalidEmail = 104001,
}

public enum eSceneType
{
    None,
    Intro,
    Patch,
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

public enum eHorizontalAlignment
{
    Left,
    Center,
    Right
}