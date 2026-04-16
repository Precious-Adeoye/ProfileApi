namespace ProfileApi.Services.Interfaces
{
    public interface IExternalApiService
    {
        Task<(string? gender, double? probability, int? sampleSize)> GetGenderAsync(string name);
        Task<(int? age, string ageGroup)> GetAgeAsync(string name);
        Task<(string? countryId, double? probability)> GetNationalityAsync(string name);



    }
}
