using System.Reflection.Metadata.Ecma335;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.TajerApi;

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

                if (update.Message.Text == "/start")
                {
                    var categories = await CategoryFetcher.FetchCategories();
                    var categoryNames = categories.Select(c => "c/"+c.Name).ToArray();
                    var keyboardButtons = categoryNames
                        .Select(categoryName => new KeyboardButton(categoryName)).ToArray();

                    var rows = SplitIntoRows(keyboardButtons, 4);
                    var keyboard = new ReplyKeyboardMarkup(rows);

                    var keyboardMarkup = CreateKeyboardWithButtons(categoryNames, 3);
                    await client.SendTextMessageAsync(message.Chat.Id, "Please select a category:",
                        replyMarkup: keyboardMarkup);
                }
                else if (update.Message.Text.StartsWith("c/"))
                {
                    var categoryName = message.Text.Remove(0, 2);
                    var category = await CategoryFetcher.GetCategoryByName(categoryName);//remove c/
                    
                    if (category == null)
                    {
                        await Reply(message, "Category not found");
                        return;
                    }

                    //await client.SendTextMessageAsync(message.Chat.Id, category.Id);
                    var items = await ItemFetcher.GetItems(category.Id);

                    if (items.Count == 0)
                    {
                        await Reply(message, $"There are no items in {categoryName} category");
                        return;
                    }
                    
                    await SendItemDetail(items.First(), categoryName, message.Chat.Id);
                }
                else if (update.Message.Text == "Next")
                {
                    
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id,
                        "Sorry, I didn't understand that command. \n You said:\n" + message.Text);
                    return;
                            //var cats = await CategoryFetcher.FetchCategories();
                            var cat = await CategoryFetcher.GetCategoryByName(message.Text);
                            if (cat == null) return;
                            ////await client.SendTextMessageAsync(message.Chat.Id, "Sorry, I didn't understand that command. \n You said:\n" + mes);
                            //await client.SendTextMessageAsync(message.Chat.Id, cat.Id);
                            await client.SendTextMessageAsync(message.Chat.Id, cat.Id);
                            var items = await ItemFetcher.GetItems(cat.Id);
                            
                            // Testing
                            var mes = string.Join(',', items.Select(i =>i.Name));
                            await client.SendTextMessageAsync(message.Chat.Id, mes);
                            
                             var itemsNames = items.Select(c =>c.Name).ToArray();
                            var keyboardButtons = itemsNames
                                .Select(itemsName => new KeyboardButton(itemsName)).ToArray();
                            
                            var rows = SplitIntoRows(keyboardButtons, 4);
                            
                            var keyboard = new ReplyKeyboardMarkup(rows);
                            
                            await client.SendTextMessageAsync(message.Chat.Id, "Please select a an item:",
                                replyMarkup: keyboard);
                            
                            await SendItemDetail(items.First(), "cate", message.Chat.Id);

                }
            } 
        }

        private async Task Reply(TelegramMessageDto message, string reply)
        {
            await client.SendTextMessageAsync(message.Chat.Id,
                reply);
        }


        // Create a separate method to send the item details
        private async Task SendItemDetail(ItemFetcher.Item item, string categoryName, long chatId)
        {
            var caption = $"{item.Name}\n${item.ItemDetails.First().Price} \n{categoryName}";
                          // Sending photo of the item
            await client.SendPhotoAsync(chatId, new InputFileUrl(item.Thumbnail), caption: caption);

            //var keyboard = CreateKeyboardWithButtons(new[] { "Previous", "Next", "Back" }, 2, item.Id);
            //await client.SendTextMessageAsync(chatId, "Choose an option:", replyMarkup: keyboard);
            
            var keyboard = CreateKeyboardWithButtons(new[] { "Previous", "Back", "Next" }, 2);
            await client.SendTextMessageAsync(chatId, "Choose an option:", replyMarkup: keyboard);

 
             // Create InlineKeyboardButton array for item details
            var inlineKeyboardButtons = new[]
            {
                InlineKeyboardButton.WithCallbackData("Buy", $"buy_{item.Id}"),
                InlineKeyboardButton.WithCallbackData("Add to wishlist", $"wishlist_{item.Id}"),
            };

            // Create the keyboard and send it
            var inlineKeyboardMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
            await client.SendTextMessageAsync(chatId, "Choose an option:", replyMarkup: inlineKeyboardMarkup);
        }
        
        public static string ExtractCategoryName(string caption)
        {
            // Split the caption by newline
            var parts = caption.Split('\n');

            // The last part is the category name
            var categoryName = parts.Last();

            return categoryName;
        }
        
        public static async Task<InputFile> ConvertUrlToInputFile(string imageUrl)
        {
            using var httpClient = new HttpClient();
            var imageData = await httpClient.GetByteArrayAsync(imageUrl);
            var imageStream = new MemoryStream(imageData);
            //InputFile img 
            return new InputFileUrl(imageUrl);
        }
        
        // Helper method to split the buttons into rows
        static KeyboardButton[][] SplitIntoRows(KeyboardButton[] buttons, int buttonsPerRow)
        {
            int numRows = (int)Math.Ceiling((double)buttons.Length / buttonsPerRow);
            KeyboardButton[][] rows = new KeyboardButton[numRows][];

            for (int i = 0; i < numRows; i++)
            {
                int start = i * buttonsPerRow;
                int end = Math.Min(start + buttonsPerRow, buttons.Length);
                rows[i] = buttons.Skip(start).Take(end - start).ToArray();
            }

            return rows;
        }
        
        private InlineKeyboardMarkup CreateKeyboardWithButtons(string[] options, int columns, string itemId)
        {
            var keyboardButtons = new List<InlineKeyboardButton>();
    
            foreach (var option in options)
            {
                keyboardButtons.Add(InlineKeyboardButton.WithCallbackData(option, $"{option}:{itemId}"));
            }

            var keyboardRows = new List<List<InlineKeyboardButton>>();

            for (var i = 0; i < keyboardButtons.Count; i += columns)
            {
                keyboardRows.Add(keyboardButtons.Skip(i).Take(columns).ToList());
            }

            return new InlineKeyboardMarkup(keyboardRows);
        }
        
        public ReplyKeyboardMarkup CreateKeyboardWithButtons(string[] buttonNames, int buttonsPerRow)
        {
            var rows = new List<KeyboardButton[]>();
            var currentRow = new List<KeyboardButton>();

            foreach (var buttonName in buttonNames)
            {
                currentRow.Add(new KeyboardButton(buttonName));

                if (currentRow.Count >= buttonsPerRow)
                {
                    rows.Add(currentRow.ToArray());
                    currentRow.Clear();
                }
            }

            // If there are remaining buttons in the last row (count less than buttonsPerRow), add them as a row
            if (currentRow.Count > 0)
            {
                rows.Add(currentRow.ToArray());
            }

            return new ReplyKeyboardMarkup(rows.ToArray());
        }

    }
}
