namespace FreeIpaClient.Exceptions
{
    /// <summary>
    ///  ???
    /// </summary>
    public class FreeIpaError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string Name { get; set; }
    }
}