namespace Voidwell.FileWell
{
    public class AppOptions
    {
        public string ContentRoot { get; set; }
        public string Authority { get; set; }
        public string ClientSecret { get; set; }
        public string SignoutRedirectUrl { get; set; }
        public string CallbackHost { get; set; }
    }
}
