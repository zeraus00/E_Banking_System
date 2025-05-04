using Exceptions;
using ViewModels.Places;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Services.DataManagement
{
    public class ApiLoaderService
    {
        private HttpClient _httpClient;

        public ApiLoaderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets all regions of the Philippines from the PSGC API 
        /// and stores them in a list of <see cref="RegionViewModel"/>.
        /// </summary>
        /// <returns>A list of <see cref="RegionViewModel"/> containing the details for each region.</returns>
        /// <exception cref="ApiException">
        /// Thrown if the API fails loading into a string or fails deserializing.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the regions list is empty.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown for other unexpected behaviors.
        /// </exception>
        public async Task<List<RegionViewModel>> GetRegionsAsync()
        {
            try
            {
                //  Get RegionViewModel list.
                //  Throws ApiException if the API fails loading data.
                //  Throws InvalidOperationException if deserialization fails.
                string apiLink = "https://psgc.gitlab.io/api/regions/";
                List<RegionViewModel> regions = await GetLocationUnitViewModelAsync<RegionViewModel>(apiLink);

                //  Thorw an InvalidOperationException if the regions list is empty.
                if (!regions.Any())
                    throw new InvalidOperationException("Regions deserialized to list. No regions in list.");

                //  Return the list of region view models.
                return regions;
            } 
            catch (ApiException)
            {
                //  Log Exception
                //  Rethrow error
                throw;
            }
            catch (InvalidOperationException)
            {
                //  Log Exception
                //  Rethrow error
                throw;
            }
            catch (Exception)
            {
                //  Log Exception
                throw;
            }
            
        }

        /// <summary>
        /// Gets all provinces under a selected region, represented with a region code
        /// and stores them in a list of <see cref="ProvinceViewModel"/>.
        /// </summary>
        /// <param name="selectedRegionCode">The region code of the selected region.</param>
        /// <returns>A list of <see cref="ProvinceViewModel"/> containing the details for each region.</returns>
        /// <exception cref="ApiException">
        /// Thrown if the API fails loading into a string or fails deserializing.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown for other unexpected behaviors.
        /// </exception>
        public async Task<List<ProvinceViewModel>> GetProvincesFromRegionAsync(string selectedRegionCode)
        {
            try
            {
                //  Get ProvinceViewModel list.
                //  Throws ApiException if the API fails loading data.
                //  Throws InvalidOperationException if deserialization fails.
                string apiLink = "https://psgc.gitlab.io/api/provinces/";
                List<ProvinceViewModel> provincesFromRegion = await GetLocationUnitViewModelAsync<ProvinceViewModel>(
                    apiLink, p => p.regionCode.ToString() == selectedRegionCode
                );

                return provincesFromRegion;
            }
            catch (ApiException)
            {
                //  Log error
                throw;
            }
            catch (Exception)
            {
                //  Log error
                throw;
            }
        }

        public async Task<List<CityViewModel>> GetCitiesMunicipalitesFromRegionAsync(string selectedRegionCode)
        {
            try
            {
                //  Get ProvinceViewModel list.
                //  Throws ApiException if the API fails loading data.
                //  Throws InvalidOperationException if deserialization fails.
                string apiLink = "https://psgc.gitlab.io/api/cities-municipalities/";
                List<CityViewModel> citiesFromRegion = await GetLocationUnitViewModelAsync<CityViewModel>(
                    apiLink, c => c.regionCode.ToString() == selectedRegionCode
                );
                
                ////  Throw an InvalidOperationException if the list of CityViewModels is empty.
                //if (!citiesFromRegion.Any())
                //    throw new InvalidOperationException(
                //        "Cities and municipalities deserialized to list. No cities or municipalities in list."
                //    );

                return citiesFromRegion;
            }
            catch (ApiException)
            {
                //  Log error
                throw;
            }
            //catch (InvalidOperationException)
            //{
            //    //  Log error
            //    throw;
            //}
            catch (Exception)
            {
                //  Log error
                throw;
            }
        }

        public async Task<List<CityViewModel>> GetCitiesMunicipalitiesFromProvinceAsync(string selectedProvinceCode)
        {

            try
            {
                //  Get ProvinceViewModel list.
                //  Throws ApiException if the API fails loading data.
                //  Throws InvalidOperationException if deserialization fails.
                string apiLink = "https://psgc.gitlab.io/api/cities-municipalities/";
                List<CityViewModel> citiesFromProvince = await GetLocationUnitViewModelAsync<CityViewModel>(
                        apiLink, c => c.provinceCode.ToString() == selectedProvinceCode
                    );

                //  Throw an InvalidOperationException if the list of CityViewModels is empty.
                //if (!citiesFromProvince.Any())
                //    throw new InvalidOperationException(
                //        "Cities and municipalities deserialized to list. No cities or municipalities in list."
                //    );

                return citiesFromProvince;
            }
            catch (ApiException)
            {
                //  Log Error
                throw;
            }
            //catch (InvalidOperationException)
            //{
            //    //  Log error
            //    throw;
            //}
            catch (Exception)
            {
                //  Log Error
                throw;
            }
        }

        public async Task<List<BarangayViewModel>> GetBarangaysFromCityAsync(string selectedCityCode)
        {
            try
            {
                //  Get ProvinceViewModel list.
                //  Throws ApiException if the API fails loading data.
                //  Throws InvalidOperationException if deserialization fails.
                string apiLink = "https://psgc.gitlab.io/api/barangays/";
                List<BarangayViewModel> barangaysFromCity = await GetLocationUnitViewModelAsync<BarangayViewModel>(
                        apiLink, b => b.cityCode.ToString() == selectedCityCode
                        || b.municipalityCode.ToString() == selectedCityCode
                    );

                //  Throw an InvalidOperationException if the list of BarangayViewModels is empty.
                //if (!barangaysFromCity.Any())
                //    throw new InvalidOperationException(
                //        "Barangays deserialized to list. No Barangays in list."
                //    );

                return barangaysFromCity;
            }
            catch (ApiException)
            {
                //  Log error
                throw;
            }
            //catch (InvalidOperationException)
            //{
            //    //  Log error
            //    throw;
            //}
            catch (Exception)
            {
                //  Log error
                throw;
            }
        }
        private async Task<List<LocationUnitViewModel>> GetLocationUnitViewModelAsync<LocationUnitViewModel>(
            string apiLink, 
            Expression<Func<LocationUnitViewModel, bool>>? predicate = null
            ) where LocationUnitViewModel : ILocationUnitViewModel
        {
            try
            {
                List<LocationUnitViewModel> viewModelList = new();

                var json = await _httpClient.GetStringAsync(apiLink);
                //  If the json string is null or empty, throw an ApiException.
                if (string.IsNullOrWhiteSpace(json))
                    throw new ApiException($"Failed loading data of type {typeof(LocationUnitViewModel).FullName} from {apiLink}.");

                //  Deserialize the json string into a list of the specified LocationUnitViewModel.
                viewModelList = JsonSerializer.Deserialize<List<LocationUnitViewModel>>(json)
                    //  Throw an ApiException if deserialization fails.
                    ?? throw new ApiException(
                        $"Failed deserializing a list of type {typeof(LocationUnitViewModel).FullName}."
                    );

                //  If there is an aditional condition, apply it to the list.
                if (predicate is not null)
                    viewModelList = viewModelList.AsQueryable().Where(predicate).ToList();

                return viewModelList;
            }
            catch (ApiException)
            {
                //  Log error
                throw;
            }
            catch (Exception)
            {
                //  Log error
                throw;
            }
        }
    }
}
