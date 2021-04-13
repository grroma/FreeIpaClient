namespace FreeIpaClient.Models
{
    public class FreeIpaUser
    {
        public string[] Uid { get; set; }
        public string[] Givenname { get; set; }
        public string[] Sn { get; set; }
        public string[] Cn { get; set; }
        public string[] Mail { get; set; }
        public string[] Mobile { get; set; }
        public string[] Ou { get; set; }
        public string[] Title { get; set; }
        public bool Stage { get; set; }
        public bool Preserved { get; set; }
    }
}