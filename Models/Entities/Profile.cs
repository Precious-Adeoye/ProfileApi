namespace ProfileApi.Models.Entities
{
    public class Profile
    {
        public Guid Id { get; set; }  // UUID v7
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public double? GenderProbability { get; set; }
        public int? SampleSize { get; set; }
        public int? Age { get; set; }
        public string? AgeGroup { get; set; }
        public string? CountryId { get; set; }
        public double? CountryProbability { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
