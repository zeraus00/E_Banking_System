using System.Text.Json;

public class CityViewModel
{
    public JsonElement code { get; set; }
    public JsonElement name { get; set; }
    public JsonElement oldName { get; set; }
    public JsonElement isCapital { get; set; }
    public JsonElement provinceCode { get; set; }
    public JsonElement districtCode { get; set; }
    public JsonElement regionCode { get; set; }
    public JsonElement islandGroupCode { get; set; }
    public JsonElement psgc10DigitCode { get; set; }
}