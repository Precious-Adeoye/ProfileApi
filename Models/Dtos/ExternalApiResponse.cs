namespace ProfileApi.Models.Dtos
{
    public class ExternalApiResponse
    {
        public record GenderizeResponse(string? Gender, double? Probability, int Count);
        public record AgifyResponse(int? Age);
        public record NationalizeResponse(List<CountryResponse> Country);
        public record CountryResponse(string country_id, double probability);
        
    }
}
