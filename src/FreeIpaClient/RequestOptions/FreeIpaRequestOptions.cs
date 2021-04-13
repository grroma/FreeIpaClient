using Newtonsoft.Json;

namespace FreeIpaClient.RequestOptions
{
    public class FreeIpaRequestOptions
    {
        public string Version { get; set; }
        public bool? All { get; set; }
        public bool? Raw { get; set; }
    }
    
    public class FreeIpaUserAddModRequestOptions : FreeIpaRequestOptionsAttr
    {
        public string Uid { get; set; }
    }

    public class FreeIpaUserRequestOptions : FreeIpaUserAddModRequestOptions
    {
        public string Givenname { get; set; }
        public string Sn { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Cn { get; set; }
        public string Mail { get; set; }
        public string Mobile { get; set; }
        public string Ou { get; set; }
        public string Title { get; set; }
        public string Telephonenumber { get; set; }
    }
    
    public class FreeIpaPasswdRequestOptions : FreeIpaRequestOptions
    {
        public string Principal { get; set; }
        public string Password { get; set; }
        public string Current_password { get; set; }
    }
    
    public class FreeIpaUserFindRequestOptions : FreeIpaRequestOptions
    {        
        public string Uid { get; set; }
        public string Mail { get; set; }
        public string Mobile { get; set; }
        public string Criteria { get; set; }

        public bool Preserved { get; set; }
    }
    
    public class FreeIpaStageUserFindRequestOptions : FreeIpaRequestOptions
    {
        public string Uid { get; set; }
        public string Mail { get; set; }
        public string Mobile { get; set; }
    }
    
    public class FreeIpaUserShowRequestOptions : FreeIpaRequestOptions
    {
        public string Uid { get; set; }
    }
    
    public class FreeIpaUserDisableRequestOptions : FreeIpaRequestOptions
    {        
        public string Uid { get; set; }     
    }
    
    public class FreeIpaUserEnableRequestOptions : FreeIpaRequestOptions
    {
        public string Uid { get; set; }
    }
    
    public class FreeIpaUserDelRequestOptions : FreeIpaRequestOptions
    {
        public string[] Uid { get; set; }

        public bool Continue { get; set; }

        public bool Preserve { get; set; } = false;
    }
    
    public class FreeIpaUserDelResult
    {
        public string[] Failed { get; set; }        
    }
    
    public class FreeIpaUserUndelRequestOptions : FreeIpaRequestOptions
    {
        public string Uid { get; set; }
    }
    
    public class FreeIpaUserUndelResult
    {
        public string[] Error { get; set; }
    }
    
    public class FreeIpaStageUserActivateRequestOptions : FreeIpaRequestOptions
    {
        public string Uid { get; set; }    
    }
}