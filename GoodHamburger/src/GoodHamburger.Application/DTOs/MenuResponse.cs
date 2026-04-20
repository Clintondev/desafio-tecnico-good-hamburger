namespace GoodHamburger.Application.DTOs;

public record MenuItemResponse(string Id, string Name, string Category, decimal Price);

public record MenuResponse(
    IEnumerable<MenuItemResponse> Sandwiches,
    IEnumerable<MenuItemResponse> Sides
);
