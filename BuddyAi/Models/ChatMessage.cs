namespace BuddyAi.Models
{
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public string? Avatar { get; set; }

        public ChatMessage(string sender, string text, string? avatar)
        {
            Sender = sender;
            Text = text;
            Avatar = avatar;
        }
    }
}
