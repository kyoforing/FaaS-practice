using DemoAPI.Enums;

namespace DemoAPI.Models
{
    public class Mail
    {
        public Provider Provider { get; set; }
        public string ReceiverMail { get; set; }
        public string Content { get; set; }
    }
}