using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FreeIpaClient.RequestOptions
{
    public class FreeIpaRequestOptions
    {
        public string Version { get; set; }
        public bool? All { get; set; }
        public bool? Raw { get; set; }

        [JsonPropertyName("no_members")]
        public bool? NoMembers { get; set; }
    }

    public sealed class FreeIpaDynamicRequestOptions : FreeIpaRequestOptions
    {
        [JsonExtensionData]
        public IDictionary<string, object> Options { get; set; } = new Dictionary<string, object>();

        public FreeIpaDynamicRequestOptions Add(string name, object value)
        {
            Options[name] = value;
            return this;
        }
    }
    
    public class FreeIpaUserAddModRequestOptions : FreeIpaRequestOptionsAttr
    {
        public string Uid { get; set; }
        public string Givenname { get; set; }
        public string Sn { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Cn { get; set; }
        public string Displayname { get; set; }
        public string Initials { get; set; }
        public string Homedirectory { get; set; }
        public string Gecos { get; set; }
        public string Loginshell { get; set; }
        public string Krbprincipalname { get; set; }
        public string Krbprincipalexpiration { get; set; }
        public string Krbpasswordexpiration { get; set; }
        public string Mail { get; set; }
        public string Userpassword { get; set; }
        public bool? Random { get; set; }
        public int? Uidnumber { get; set; }
        public int? Gidnumber { get; set; }
        public string Street { get; set; }
        public string L { get; set; }
        public string St { get; set; }
        public string Postalcode { get; set; }
        public string Telephonenumber { get; set; }
        public string Mobile { get; set; }
        public string Pager { get; set; }
        public string Facsimiletelephonenumber { get; set; }
        public string Ou { get; set; }
        public string Title { get; set; }
        public string Manager { get; set; }
        public string Carlicense { get; set; }
        public string[] Ipasshpubkey { get; set; }
        public string[] Ipauserauthtype { get; set; }
        public string[] Userclass { get; set; }
        public string Ipatokenradiusconfiglink { get; set; }
        public string Ipatokenradiususername { get; set; }
        public string Ipaidpconfiglink { get; set; }
        public string Ipaidpsub { get; set; }
        public string Departmentnumber { get; set; }
        public string Employeenumber { get; set; }
        public string Employeetype { get; set; }
        public string Preferredlanguage { get; set; }
        public string[] Usercertificate { get; set; }
        public bool? Nsaccountlock { get; set; }
        public string Ipantlogonscript { get; set; }
        public string Ipantprofilepath { get; set; }
        public string Ipanthomedirectory { get; set; }
        public string Ipanthomedirectorydrive { get; set; }
        public bool? Rights { get; set; }
        public string Rename { get; set; }
    }

    public class FreeIpaUserRequestOptions : FreeIpaUserAddModRequestOptions
    {
        public bool? Noprivate { get; set; }

        [JsonPropertyName("from_delete")]
        public bool? FromDelete { get; set; }
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
        public string Givenname { get; set; }
        public string Sn { get; set; }
        public string Cn { get; set; }
        public string Displayname { get; set; }
        public string Initials { get; set; }
        public string Homedirectory { get; set; }
        public string Gecos { get; set; }
        public string Loginshell { get; set; }
        public string Krbprincipalname { get; set; }
        public string Krbprincipalexpiration { get; set; }
        public string Krbpasswordexpiration { get; set; }
        public string Mail { get; set; }
        public string Userpassword { get; set; }
        public int? Uidnumber { get; set; }
        public int? Gidnumber { get; set; }
        public string Street { get; set; }
        public string L { get; set; }
        public string St { get; set; }
        public string Postalcode { get; set; }
        public string Telephonenumber { get; set; }
        public string Mobile { get; set; }
        public string Pager { get; set; }
        public string Facsimiletelephonenumber { get; set; }
        public string Ou { get; set; }
        public string Title { get; set; }
        public string Manager { get; set; }
        public string Carlicense { get; set; }
        public string[] Ipauserauthtype { get; set; }
        public string[] Userclass { get; set; }
        public string Ipatokenradiusconfiglink { get; set; }
        public string Ipatokenradiususername { get; set; }
        public string Ipaidpconfiglink { get; set; }
        public string Ipaidpsub { get; set; }
        public string Departmentnumber { get; set; }
        public string Employeenumber { get; set; }
        public string Employeetype { get; set; }
        public string Preferredlanguage { get; set; }
        public string[] Usercertificate { get; set; }
        public string Ipantlogonscript { get; set; }
        public string Ipantprofilepath { get; set; }
        public string Ipanthomedirectory { get; set; }
        public string Ipanthomedirectorydrive { get; set; }
        public bool? Nsaccountlock { get; set; }
        public string Criteria { get; set; }

        public bool? Preserved { get; set; }
        public int? Timelimit { get; set; }
        public int? Sizelimit { get; set; }
        public bool? Whoami { get; set; }

        [JsonPropertyName("pkey_only")]
        public bool? PkeyOnly { get; set; }

        [JsonPropertyName("in_group")]
        public string[] InGroup { get; set; }

        [JsonPropertyName("not_in_group")]
        public string[] NotInGroup { get; set; }

        [JsonPropertyName("in_netgroup")]
        public string[] InNetgroup { get; set; }

        [JsonPropertyName("not_in_netgroup")]
        public string[] NotInNetgroup { get; set; }

        [JsonPropertyName("in_role")]
        public string[] InRole { get; set; }

        [JsonPropertyName("not_in_role")]
        public string[] NotInRole { get; set; }

        [JsonPropertyName("in_hbacrule")]
        public string[] InHbacrule { get; set; }

        [JsonPropertyName("not_in_hbacrule")]
        public string[] NotInHbacrule { get; set; }

        [JsonPropertyName("in_sudorule")]
        public string[] InSudorule { get; set; }

        [JsonPropertyName("not_in_sudorule")]
        public string[] NotInSudorule { get; set; }

        [JsonPropertyName("in_subid")]
        public string[] InSubid { get; set; }

        [JsonPropertyName("not_in_subid")]
        public string[] NotInSubid { get; set; }
    }
    
    public class FreeIpaStageUserFindRequestOptions : FreeIpaUserFindRequestOptions { }
    
    public class FreeIpaUserShowRequestOptions : FreeIpaRequestOptions
    {
        public string Uid { get; set; }
        public bool? Rights { get; set; }
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

    public class FreeIpaEnvRequestOptions : FreeIpaRequestOptions
    {
        public bool? Server { get; set; }
    }
}
