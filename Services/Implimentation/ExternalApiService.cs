using ProfileApi.Services.Interfaces;
using static ProfileApi.Models.Dtos.ExternalApiResponse;

namespace ProfileApi.Services.Implimentation
{
    public class ExternalApiService: IExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(int? age, string ageGroup)> GetAgeAsync(string name)
        {
            var response = await _httpClient.GetFromJsonAsync<AgifyResponse>($"https://api.agify.io?name={name}");

            if (response?.Age == null)
                throw new Exception("Agify returned an invalid response");

            string ageGroup = response.Age switch
            {
                <= 12 => "child",
                <= 19 => "teenager",
                <= 59 => "adult",
                _ => "senior"
            };

            return (response.Age, ageGroup);
        }

        public async Task<(string? gender, double? probability, int? sampleSize)> GetGenderAsync(string name)
        {
            var response = await _httpClient.GetFromJsonAsync<GenderizeResponse>($"https://api.genderize.io?name={name}");

            if (response?.Gender == null || response.Count == 0)
                throw new Exception("Genderize returned an invalid response");

            return (response.Gender, response.Probability, response.Count);
        }

        public async Task<(string? countryId, double? probability)> GetNationalityAsync(string name)
        {
            var response = await _httpClient.GetFromJsonAsync<NationalizeResponse>($"https://api.nationalize.io?name={name}");

            if (response?.Country == null || !response.Country.Any())
                throw new Exception("Nationalize returned an invalid response");

            var topCountry = response.Country.OrderByDescending(c => c.Probability).First();
            return (topCountry.CountryId, topCountry.Probability);
        }
    }
}
