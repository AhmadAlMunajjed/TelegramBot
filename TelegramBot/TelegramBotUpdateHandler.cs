using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class TelegramBotUpdateHandler
    {
        private readonly ITelegramBotClient client;
        public TelegramBotUpdateHandler(ITelegramBotClient botClient)
        {
            client = botClient;
        }

        public async Task HandleUpdateAsync(TelegramUpdateDto update)
        {
            if (update.Type == UpdateType.Message || update.Type == UpdateType.Unknown)
            {
                var message = update.Message;
                if (message == null) { return; }
                if (message.Type == MessageType.Text || message.Type == MessageType.Unknown)
                {
                    switch (update.Message.Text)
                    {
                        case "/start":
                            var keyboard = new ReplyKeyboardMarkup(new[]
                            {
                                new[]
                                {
                                    new KeyboardButton("Category 1"),
                                    new KeyboardButton("Category 2"),
                                }
                            });
                            await client.SendTextMessageAsync(message.Chat.Id, "Please select a category:", replyMarkup: keyboard);
                            break;

                        case "Category 1":
                            await client.SendTextMessageAsync(message.Chat.Id, "You selected Category 1.");
                            break;

                        case "Category 2":
                            await client.SendTextMessageAsync(message.Chat.Id, "You selected Category 2.");
                            break;

                        default:
                            await client.SendTextMessageAsync(message.Chat.Id, "Sorry, I didn't understand that command. \n You said:\n" + message.Text);
                            break;
                    }
                }
            }

            // Optionally handle other types of updates
            // else if (update.Type == UpdateType.CallbackQuery)
            // {
            //     // Handle callback query
            // }
        }
    }
}
