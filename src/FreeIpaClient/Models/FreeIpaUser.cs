using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FreeIpaClient.Models
{
    public class FreeIpaUser
    {
        public string[] Uid { get; set; }
        public string[] Givenname { get; set; }
        public string[] Sn { get; set; }
        public string[] Cn { get; set; }
        public string[] Displayname { get; set; }
        public string[] Initials { get; set; }
        public string[] Mail { get; set; }
        public string[] Mobile { get; set; }
        public string[] Ou { get; set; }
        public string[] Title { get; set; }
        public string[] Uidnumber { get; set; }
        public string[] Gidnumber { get; set; }
        public string[] Homedirectory { get; set; }
        public string[] Gecos { get; set; }
        public string[] Loginshell { get; set; }
        public string[] Krbprincipalname { get; set; }
        public string[] Krbprincipalexpiration { get; set; }
        public string[] Krbpasswordexpiration { get; set; }
        public string[] Street { get; set; }
        public string[] L { get; set; }
        public string[] St { get; set; }
        public string[] Postalcode { get; set; }
        public string[] Telephonenumber { get; set; }
        public string[] Pager { get; set; }
        public string[] Facsimiletelephonenumber { get; set; }
        public string[] Manager { get; set; }
        public string[] Carlicense { get; set; }
        public string[] Ipasshpubkey { get; set; }
        public string[] Ipauserauthtype { get; set; }
        public string[] Userclass { get; set; }
        public string[] Ipatokenradiusconfiglink { get; set; }
        public string[] Ipatokenradiususername { get; set; }
        public string[] Ipaidpconfiglink { get; set; }
        public string[] Ipaidpsub { get; set; }
        public string[] Departmentnumber { get; set; }
        public string[] Employeenumber { get; set; }
        public string[] Employeetype { get; set; }
        public string[] Preferredlanguage { get; set; }
        public string[] Usercertificate { get; set; }
        public string[] Objectclass { get; set; }
        public string Dn { get; set; }

        [JsonProperty("has_password")]
        public bool[] HasPassword { get; set; }

        [JsonProperty("has_keytab")]
        public bool[] HasKeytab { get; set; }

        [JsonProperty("memberof_group")]
        public string[] MemberofGroup { get; set; }

        [JsonProperty("memberofindirect_group")]
        public string[] MemberofindirectGroup { get; set; }

        [JsonProperty("memberof_role")]
        public string[] MemberofRole { get; set; }

        [JsonProperty("memberof_hbacrule")]
        public string[] MemberofHbacrule { get; set; }

        [JsonProperty("memberof_sudorule")]
        public string[] MemberofSudorule { get; set; }

        [JsonProperty("memberof_subid")]
        public string[] MemberofSubid { get; set; }

        [JsonProperty("memberindirect_group")]
        public string[] MemberindirectGroup { get; set; }

        [JsonProperty("memberindirect_role")]
        public string[] MemberindirectRole { get; set; }

        public bool Stage { get; set; }
        public bool Preserved { get; set; }
        public List<bool> Nsaccountlock { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }
    }
}
