using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.AspNetCore.Http;
using Telegram.Bot.Types.Enums;
using System.Threading;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class TelegramBotService
    {
        readonly string baseApiUrl = "6163649653:AAFhd8bo8MsR5i5Hou9Z6G0wYEj7XrA_pDI"; // TODO: to use public key, change url here
        readonly string[] tenants = new string[] { "6163649653:AAFhd8bo8MsR5i5Hou9Z6G0wYEj7XrA_pDI" }; // TODO: place your key here
        public TelegramBotService()
        {

        }

        public async Task RegisterWebhookAsync(int tenantId)
        {
            // get botToken by tenantId
            string botToken = tenants[tenantId];
            var botClient = new TelegramBotClient(botToken);
            var webhookStatus = botClient.SetWebhookAsync($"{baseApiUrl}/api/telegram/updates/{tenantId}");
            Console.WriteLine($"Webhook Status: {webhookStatus}");
        }

        public async Task HandleUpdateAsync(int tenantId, TelegramUpdateDto update)
        {
            // get botToken by tenantId
            string botToken = tenants[tenantId];
            var botClient = new TelegramBotClient(botToken);

            var handler = new TelegramBotUpdateHandler(botClient);
            await handler.HandleUpdateAsync(update);
        }

        public async Task StartPoolingAsync(int tenantId)
        {
            string botToken = tenants[tenantId];
            var botClient = new TelegramBotClient(botToken);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };

            try
            {
                await botClient.ReceiveAsync(
                HandlePoolingUpdateAsync,
                HandlePoolingErrorAsync,
                    receiverOptions,
                    cancellationToken
                );
            }
            catch (Exception exception)
            {

            }
        }

        private async Task HandlePoolingUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is Message message)
            {
                var handler = new TelegramBotUpdateHandler(botClient);
                await handler.HandleUpdateAsync(new TelegramUpdateDto()
                {
                    update_id = update.Id,
                    Message = new TelegramMessageDto()
                    {
                        Date = message.Date.Ticks,
                        Chat = new TelegramChatDto()
                        {
                            Id = message.Chat.Id,
                            first_name = message.Chat.FirstName,
                            last_name = message.Chat.LastName,
                            username = message.Chat.Username,
                        },
                        Message_id = message.MessageId,
                        Text = message.Text,
                        Type = message.Type,
                        From = new TelegramUserDto()
                        {
                            Id = message.From.Id,
                            first_name = message.From.FirstName,
                            last_name = message.From.LastName,
                            username = message.From.Username,
                        }
                    }
                });
            }
        }

        private async Task HandlePoolingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                await botClient.SendTextMessageAsync(123, apiRequestException.ToString());
            }
        }
    }
}
