namespace FreeIpaClient.Constants
{
    /// <summary>
    /// Supported FreeIpaClient API methods.
    /// <remarks>
    /// See  <see chref="https://ipa.demo1.freeipa.org/ipa/ui/#/p/apibrowser/type=command"/> a complete list.
    /// login: admin
    /// password: Secret123
    /// </remarks>
    /// </summary>
    public static class FreeIpaApiMethods
    {
        public const string Ping = "ping";
        public const string UserFind = "user_find";
        public const string UserAdd = "user_add";
        public const string UserMod = "user_mod";
        public const string StageUserFind = "stageuser_find";
        public const string StageUserAdd = "stageuser_add";
        public const string StageUserMod = "stageuser_mod";
        public const string Passwd = "passwd";
        public const string UserShow = "user_show";
        public const string UserDisable = "user_disable";
        public const string UserEnable = "user_enable";
        public const string UserDel = "user_del";
        public const string UserUndel = "user_undel";
        public const string StageUserDel = "stageuser_del";
        public const string StageUserActivate = "stageuser_activate";
    }
}