using System.Text.Json;

namespace ViewModels.Places
{
    public interface ILocationUnitViewModel
    {
        JsonElement code { get; set; }
        JsonElement name { get; set; }
        JsonElement islandGroupCode { get; set; }
        JsonElement psgc10DigitCode { get; set; }
    }
}
