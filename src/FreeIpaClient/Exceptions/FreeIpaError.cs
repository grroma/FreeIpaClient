namespace FreeIpaClient.Exceptions
{
    public class FreeIpaError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string Name { get; set; }
    }
}