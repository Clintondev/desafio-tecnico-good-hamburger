using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class OrderApiService
{
    private readonly HttpClient _http;

    public OrderApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<OrderModel>?> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<OrderModel>>("api/orders");
    }

    public async Task<OrderModel?> GetByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<OrderModel>($"api/orders/{id}");
    }

    public async Task<OrderModel?> CreateAsync(CreateOrderModel model)
    {
        var response = await _http.PostAsJsonAsync("api/orders", model);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderModel>();
    }

    public async Task<OrderModel?> UpdateAsync(Guid id, CreateOrderModel model)
    {
        var response = await _http.PutAsJsonAsync($"api/orders/{id}", model);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderModel>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/orders/{id}");
        response.EnsureSuccessStatusCode();
    }
}
