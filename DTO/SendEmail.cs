namespace DTO
{
    public class SendEmail
    {
        public string EmailAddres { get; set; }
        public string Subject { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }
    }
}
