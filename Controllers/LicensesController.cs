using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Data;
using TaskFleet.Models;

namespace TaskFleet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LicensesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LicensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLicenses()
        {
            var licenses = await _context.Licenses.ToListAsync();
            return Ok(licenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLicenseById(int id)
        {
            var license = await _context.Licenses.FindAsync(id);

            if (license == null)
                return NotFound(new { Message = $"License with ID {id} not found." });

            return Ok(license);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLicense([FromBody] License license)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Licenses.Add(license);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLicenseById), new { id = license.LicenseId }, license);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLicense(int id, [FromBody] License updatedLicense)
        {
            if (id != updatedLicense.LicenseId)
                return BadRequest(new { Message = "License ID mismatch." });

            var existingLicense = await _context.Licenses.FindAsync(id);

            if (existingLicense == null)
                return NotFound(new { Message = $"License with ID {id} not found." });

            existingLicense.Category = updatedLicense.Category;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLicense(int id)
        {
            var license = await _context.Licenses.FindAsync(id);

            if (license == null)
                return NotFound(new { Message = $"License with ID {id} not found." });

            _context.Licenses.Remove(license);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
