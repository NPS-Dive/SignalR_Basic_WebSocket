namespace SignalR.Models;

public class ChatMessage
{
    public ChatMessage ( string name, string sentAt, string text )
    {
        SenderName = name;
        SentAt = DateTime.Parse(sentAt);
        Text = text;
    }

    public string SenderName { get; set; }
    public string Text { get; set; }
    public DateTime SentAt { get; set; }
}