namespace Scrapping.Models
{
    public class SeleniumConfig
    {
        public string? UrlPagina { get; set; }
        public int Timeout { get; set; }
        public int Headless { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
