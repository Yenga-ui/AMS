using AssetManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly AssetContext _context;
        public StatsController(AssetContext assetContext ) {

            _context = assetContext;
        
        }



        [HttpGet("stats")]
        public async Task<IActionResult> getAllStats()
        {
            int stockCount= await _context.DtStocks.CountAsync();
             int assignedStockCount = await _context.DtStocks.CountAsync(s => s.Assigned == 1);
            int borrowedStockCount= await _context.DtStocks.CountAsync(s=>s.Assigned == 2);
            int maintainanceStock= await _context.AppMaitenances.CountAsync();
            return Ok(
new
{
    total = stockCount,
    assigned = assignedStockCount,
    borrowed = borrowedStockCount,
    maintainance = maintainanceStock
});

        }
    }
}
