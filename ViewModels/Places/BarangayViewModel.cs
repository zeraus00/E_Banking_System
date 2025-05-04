using System.Text.Json;

namespace ViewModels.Places
{
    public class BarangayViewModel : ILocationUnitViewModel
    {
        public JsonElement code { get; set; }
        public JsonElement name { get; set; }
        public JsonElement oldName { get; set; }
        public JsonElement subMunicipalityCode { get; set; }
        public JsonElement cityCode { get; set; }
        public JsonElement municipalityCode { get; set; }
        public JsonElement districtCode { get; set; }
        public JsonElement provinceCode { get; set; }
        public JsonElement regionCode { get; set; }
        public JsonElement islandGroupCode { get; set; }
        public JsonElement psgc10DigitCode { get; set; }
    }
}
    

