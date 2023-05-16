using TelegramBot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<TelegramBotService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("app/pooling/start", async (TelegramBotService telegramBotService) =>
{
    await telegramBotService.StartPoolingAsync(0);
});

app.MapGet("/api/telegram/register/{tenantId}", async (int tenantId, TelegramBotService telegramBotService) =>
{
    await telegramBotService.RegisterWebhookAsync(tenantId);
});

app.MapPost("/api/telegram/updates/{tenantId}", async (int tenantId, TelegramUpdateDto update, TelegramBotService telegramBotService) =>
{
    await telegramBotService.HandleUpdateAsync(tenantId, update);
});

app.Run();

