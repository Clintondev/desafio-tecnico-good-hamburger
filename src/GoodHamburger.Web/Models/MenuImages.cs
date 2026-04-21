namespace GoodHamburger.Web.Models;

public static class MenuImages
{
    public static string Get(string itemId)
    {
        return itemId switch
        {
            "x-burger" => "images/menu/x-burger.png",
            "x-egg" => "images/menu/x-egg.png",
            "x-bacon" => "images/menu/x-bacon.png",
            "fries" => "images/menu/fries.png",
            "drink" => "images/menu/drink.png",
            _ => "images/menu/x-burger.png"
        };
    }

    public static string Get(SandwichType type)
    {
        return type switch
        {
            SandwichType.XBurger => Get("x-burger"),
            SandwichType.XEgg => Get("x-egg"),
            SandwichType.XBacon => Get("x-bacon"),
            _ => Get("x-burger")
        };
    }
}
