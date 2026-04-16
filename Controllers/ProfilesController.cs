using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileApi.Data;
using ProfileApi.Models.Entities;
using ProfileApi.Services.Interfaces;

namespace ProfileApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IExternalApiService _apiService;

        public ProfilesController(AppDbContext context, IExternalApiService apiService)
        {
            _context = context;
            _apiService = apiService;
        }

        // POST /api/profiles
        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
        {
            // Validation
            if (request?.Name == null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { status = "error", message = "Missing or empty name" });

            // Check for existing profile (Idempotency)
            var existing = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Name.ToLower() == request.Name.ToLower());

            if (existing != null)
            {
                return Ok(new
                {
                    status = "success",
                    message = "Profile already exists",
                    data = existing
                });
            }

            try
            {
                // Call external APIs
                var (gender, genderProb, sampleSize) = await _apiService.GetGenderAsync(request.Name);
                var (age, ageGroup) = await _apiService.GetAgeAsync(request.Name);
                var (countryId, countryProb) = await _apiService.GetNationalityAsync(request.Name);

                // Create profile
                var profile = new Profile
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Gender = gender ?? "unknown",
                    GenderProbability = genderProb ?? 0,
                    SampleSize = sampleSize ?? 0,
                    Age = age ?? 0,
                    AgeGroup = ageGroup ?? "unknown",
                    CountryId = countryId?.ToUpper() ?? "unknown",
                    CountryProbability = countryProb ?? 0,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { status = "success", data = profile });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Genderize"))
                    return StatusCode(502, new { status = "error", message = "Genderize returned an invalid response" });
                if (ex.Message.Contains("Agify"))
                    return StatusCode(502, new { status = "error", message = "Agify returned an invalid response" });
                if (ex.Message.Contains("Nationalize"))
                    return StatusCode(502, new { status = "error", message = "Nationalize returned an invalid response" });

                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        // GET /api/profiles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var profile = await _context.Profiles.FindAsync(id);

            if (profile == null)
                return NotFound(new { status = "error", message = "Profile not found" });

            return Ok(new { status = "success", data = profile });
        }

        // GET /api/profiles
        [HttpGet]
        public async Task<IActionResult> GetAllProfiles(
            [FromQuery] string? gender,
            [FromQuery] string? country_id,
            [FromQuery] string? age_group)
        {
            var query = _context.Profiles.AsQueryable();

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(p => p.Gender != null && p.Gender.ToLower() == gender.ToLower());

            if (!string.IsNullOrEmpty(country_id))
                query = query.Where(p => p.CountryId != null && p.CountryId.ToUpper() == country_id.ToUpper());

            if (!string.IsNullOrEmpty(age_group))
                query = query.Where(p => p.AgeGroup != null && p.AgeGroup.ToLower() == age_group.ToLower());

            var profiles = await query
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Gender,
                    p.Age,
                    p.AgeGroup,
                    p.CountryId
                })
                .ToListAsync();

            return Ok(new
            {
                status = "success",
                count = profiles.Count,
                data = profiles
            });
        }

        // DELETE /api/profiles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(Guid id)
        {
            var profile = await _context.Profiles.FindAsync(id);

            if (profile == null)
                return NotFound(new { status = "error", message = "Profile not found" });

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

public class CreateProfileRequest
{
    public string Name { get; set; } = string.Empty;
}
