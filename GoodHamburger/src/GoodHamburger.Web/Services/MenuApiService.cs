using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class MenuApiService
{
    private readonly HttpClient _http;

    public MenuApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<MenuModel?> GetMenuAsync()
    {
        return await _http.GetFromJsonAsync<MenuModel>("api/menu");
    }
}
