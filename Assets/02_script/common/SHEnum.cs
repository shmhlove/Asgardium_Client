
public enum SHErrorCode
{
    // network error (Sync Server)
    HTTP = 1000,
    Common_InvalidParam = 1001,
    Common_DatabaseFind = 1002,
    Common_DatabaseWrite = 1003,
    Common_InvalidResponseData = 1004,
    Common_JsonParse = 1005,

    Auth_UserFind = 2001,
    Auth_AlreadySignupUser = 2002,
}