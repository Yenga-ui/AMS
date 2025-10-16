using AssetManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementSystem.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SystemController : ControllerBase

    {
        private readonly AssetContext _context;

        public SystemController(AssetContext context)
        {
            _context = context;
        }
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.DtCategories.ToListAsync();
              
            return Ok(categories);
        }
        [HttpGet("TagData")]
        public async Task<IActionResult> getTagInformation([FromQuery] string assetNumber)
        {
            if (string.IsNullOrEmpty(assetNumber))
            {
                return BadRequest("Asset number is required.");
            }

            // Fetch the asset from the database using the asset number
            var asset = await _context.DtStocks
                .Include(x => x.Project)  // Include the related Project entity
                .Include(x => x.Location) // Include the related Location entity
                .FirstOrDefaultAsync(x => x.Itnumber == assetNumber); // Assuming AssetNumber is the property name

            if (asset == null)
            {
                return NotFound("Asset not found.");
            }

            // Ensure that Project and Location are not null, and return them in the response
            return Ok(new
            {
                SerialNo = asset.SerialNo,
                Description = asset.Description,
                Project = asset.Project != null ? asset.Project.Project : "No project",  // Handle null project
                Location = asset.Location != null ? asset.Location.Location : "No location"  // Handle null location
            });
        }



        [HttpGet("projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _context.DtProjects.ToListAsync();
            return Ok(projects);
        }

        [HttpGet("conditions")]
        public async Task<IActionResult> GetAllConditions()
        {
            var conditions = await _context.DtConditions.ToListAsync();
            return Ok(conditions);
        }

        [HttpGet("locations")] 
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _context.DtLocations.ToListAsync();
            return Ok(locations);
        }
        [HttpGet("stockstatuses")]
        public async Task<IActionResult> GetAllStockStatus()
        {
            var stockStatus = await _context.DtStatuses.ToListAsync();
            return Ok(stockStatus);
        }
        [HttpGet("currencies")]
        public async Task<IActionResult> GetAllCurrencies()
        {
            var currencies = await _context.DtCurrencies.ToListAsync();
            return Ok(currencies);
        }
    }
}
