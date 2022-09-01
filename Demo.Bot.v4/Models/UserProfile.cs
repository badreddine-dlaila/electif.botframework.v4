using System;

namespace Demo.Bot.v4.Models
{
    public class UserProfile
    {
        public string Name  { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }

        public string   Description  { get; set; }
        public DateTime CallbackTime { get; set; }
        public string   PhoneNumber  { get; set; }
        public string   Bug          { get; set; }
    }
}
