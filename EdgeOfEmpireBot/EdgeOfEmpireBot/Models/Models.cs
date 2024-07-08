namespace HK47.Models;

/// <summary>
/// A model that represents the game details that we store
/// </summary>
public class SteamGameDetails
{
    public required string Name { get; set; }

    public required string AppID { get; set; }

    public required string Price { get; set; }
}