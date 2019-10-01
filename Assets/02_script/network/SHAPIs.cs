using System;

public static class SHAPIs
{
    // Web : 테스트 API
    public static string SH_API_RETRY_REQUEST = "/process/test";
    public static string SH_API_TEST_USE_POWER = "/process/test_use_mining_power";
    public static string SH_API_TEST_RESET_POWER = "/process/test_reset_mining_power";
    
    // Web : 인증 관련
    public static string SH_API_AUTH_IS_SIGNUP = "/process/is_signup";
    public static string SH_API_AUTH_SIGNUP = "/process/signup";
    public static string SH_API_AUTH_SIGNIN = "/process/signin";

    // Web : User 관련
    public static string SH_API_USER_GET_INVENTORY = "/process/instance_user_inventory";

    // Web : Mining 관련
    public static string SH_API_MINING_PURCHASE_ACTIVE = "/process/purchase_unit_at_mining_active";
    
    // Web : 테이블 관련
    public static string SH_API_GET_CONFIG_TABLE = "/table/global_config";
    public static string SH_API_GET_UNIT_DATA_TABLE = "/table/global_unit_data";
    public static string SH_API_GET_MINING_ACTIVE_COMPANY_NPC_TABLE = "/table/mining_active_company_npc";
    public static string SH_API_GET_MINING_ACTIVE_COMPANY_TABLE = "/table/instance_mining_active_company";
    public static string SH_API_GET_MINING_ACTIVE_QUANTITY_TABLE = "/table/mining_active_quantity";
    public static string SH_API_GET_MINING_ACTIVE_SUPPLY_TABLE = "/table/mining_active_supply";

    // Socket : 테스트 API
    public static string SH_SOCKET_REQ_TEST = "test_message";
    public static string SH_SOCKET_REQ_FORCE_DISCONNECT = "force_disconnect";

    // Socket : MiningActiveCompany 정보 구독/구독해제 요청
    public static string SH_SOCKET_REQ_SUBSCRIBE_MINING_ACTIVE_INFO = "subscribe_mining_active_info";
    public static string SH_SOCKET_REQ_UNSUBSCRIBE_MINING_ACTIVE_INFO = "unsubscribe_mining_active_info";

    // Socket : 폴링 이벤트
    public static string SH_SOCKET_POLLING_MINING_ACTIVE_INFO = "polling_mining_active_info";
}
