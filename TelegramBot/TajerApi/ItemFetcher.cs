using Newtonsoft.Json;

namespace TelegramBot.TajerApi;

public class ItemFetcher
{
    public class ItemDetail
    {
        public string Id { get; set; }
        public string Sku { get; set; }
        public int Calories { get; set; }
        public int Price { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public List<ItemDetail> ItemDetails { get; set; }
    }

    public class Root
    {
        public List<Item> Items { get; set; }
    }

    public static async Task<List<Item>> GetItems(string categoryId)
    {
        string url =
            $"https://api.tajer.tech/api/app/item/item-store?skipCount=0&maxResultCount=10&warehouseId=d80f9604-1d58-21c3-40ba-39fe8ac34042&branchId=3108b697-a2c6-58a8-ed15-39fe8ac33fcc&categoryId={categoryId}";

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("tajertenant", "d46c2a26-b508-abe7-6b32-39fe8ac2f335");

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                Root apiResponse = JsonConvert.DeserializeObject<Root>(content);

                return apiResponse?.Items;
            }
            else
            {
                throw new Exception($"Failed to get items. Status code: {response.StatusCode}");
            }
        }
    }
}