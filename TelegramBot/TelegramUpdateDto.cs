using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
    public class TelegramUpdateDto
    {
        public UpdateType Type { get; set; }
        public int update_id { get; set; }
        public TelegramMessageDto Message { get; set; }
    }

    public class TelegramMessageDto
    {
        public int Message_id { get; set; }
        public TelegramUserDto From { get; set; }
        public TelegramChatDto Chat { get; set; }
        public MessageType Type { get; set; }
        public string Text { get; set; }
        public long Date { get; set; }
    }

    public class TelegramUserDto
    {
        public long Id { get; set; }
        public bool Is_bot { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
    }

    public class TelegramChatDto
    {
        public long Id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string Type { get; set; }
    }
}
