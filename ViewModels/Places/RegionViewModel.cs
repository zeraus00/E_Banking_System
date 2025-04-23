using System.Text.Json;

public class RegionViewModel
{
    public JsonElement code { get; set; }
    public JsonElement name { get; set; }
    public JsonElement regionName { get; set; }
    public JsonElement islandGroupCode { get; set; }
    public JsonElement psgc10DigitCode { get; set; }
}