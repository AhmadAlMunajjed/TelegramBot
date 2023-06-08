using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class Category
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class CategoryFetcher
{
    private static HttpClient client = new HttpClient();
    private const string url = "https://api.tajer.tech/api/app/category/all";
    private const string tajertenant = "d46c2a26-b508-abe7-6b32-39fe8ac2f335";

    public static async Task<Category?> GetCategoryByName(string name)
    {
        var categories = await FetchCategories();
        var cat =  categories.FirstOrDefault(c => c.Name == name);
        return cat;
    }

    public static async Task<Category> GetCategoryById(string id)
    {
        var categories = await FetchCategories();
        var cat = categories.FirstOrDefault(c => c.Id == id);
        return cat;
    }

    public static async Task<List<Category>> FetchCategories()
    {
        client = new HttpClient();//don't remove it, it'll prevent re-adding headers.
        client.DefaultRequestHeaders.Add("tajertenant", tajertenant);

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var contentString = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(contentString);
            var items = (JArray)data["items"];

            var categories = new List<Category>();

            foreach (var item in items)
            {
                categories.Add(new Category
                {
                    Id = (string)item["id"],
                    Name = (string)item["name"],
                });
            }

            return categories;
        }
        else
        {
            Console.WriteLine($"Error retrieving data. Status code: {response.StatusCode}");
            return new List<Category>();
        }
    }
}
