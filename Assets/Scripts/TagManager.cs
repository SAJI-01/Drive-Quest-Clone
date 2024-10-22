public enum VehicleType
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Orange,
    Pink
}
public static class TagManager
{
    private static readonly string[] CarTags = {
        "RedCar",
        "BlueCar",
        "GreenCar",
        "YellowCar",
        "PurpleCar",
        "OrangeCar",
        "PinkCar"
    };
    
    private static readonly string[] ShipTags = {
        "RedShip",
        "BlueShip",
        "GreenShip",
        "YellowShip",
        "PurpleShip",
        "OrangeShip",
        "PinkShip"
    };
    
    public static bool IsValidCarTag(string tag)
    {
        return System.Array.Exists(CarTags, value => value == tag);
    }

    public static bool IsValidShipTag(string tag)
    {
        return System.Array.Exists(ShipTags, value => value == tag);
    }
}