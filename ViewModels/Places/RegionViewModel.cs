using System.Text.Json;

namespace ViewModels.Places
{
    public class RegionViewModel : ILocationUnitViewModel
    {
        public JsonElement code { get; set; }
        public JsonElement name { get; set; }
        public JsonElement regionName { get; set; }
        public JsonElement islandGroupCode { get; set; }
        public JsonElement psgc10DigitCode { get; set; }
    }
}