using AssetManagementSystem.DTO;
using AssetManagementSystem.Models;
using AssetManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Formats.Asn1;

namespace AssetManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly AssetContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public StockController(AssetContext context, IConfiguration configuration,IEmailService service)
        {
            _context = context;
            _configuration = configuration;
            _emailService = service;
        }

        [HttpGet("stocks")]
        public async Task<IActionResult> GetAllStocks()
        
        {
            var stocks = await _context.DtStocks
    .Where(s => s.Assigned == 0)
    .OrderByDescending(s => s.Id)
    .Select(s => new {
        s.Id,
        s.SerialNo,
        s.Make,
        s.Model,
        s.Itnumber
    })
    .ToListAsync();

            return Ok(stocks);
        }

        [HttpGet("AssignedOrBorrowed")]
        public async Task<IActionResult> GetABStocks()
        {
            var stocks = await _context.DtStocks
        .Where(s => s.Assigned == 1 || s.Assigned == 2)
        .OrderByDescending(s=>s.Id)
        .Select(s => new {
            s.Id,
            s.SerialNo,
            s.Make,
            s.Model,
            s.Itnumber
        })
         .ToListAsync();

            return Ok(stocks);

        }

        [HttpGet("Assigned")]
        public async Task<IActionResult> GetAssignedStocks()
        {
            var stocks = await _context.DtStocks
        .Where(s => s.Assigned == 1 || s.Assigned == 2)
        .OrderByDescending(s => s.Id)
        .Select(s => new {
            s.Id,
            s.SerialNo,
            s.Make,
            s.Model,
            s.Itnumber
        })
         .ToListAsync();

            return Ok(stocks);

        }
        [HttpGet("Borrowed")]
        public async Task<IActionResult> GetBorrowedStocks()
        {
            var stocks = await _context.DtStocks
        .Where(s => s.Assigned == 1 || s.Assigned == 2)
        .OrderByDescending(s => s.Id)
        .Select(s => new {
            s.Id,
            s.SerialNo,
            s.Make,
            s.Model,
            s.Itnumber
        })
         .ToListAsync();

            return Ok(stocks);

        }



        [HttpGet("warehouseserials")]
        public async Task<IActionResult> getInWarehouseSerials()
        {
               var serials = await _context.DtStocks
                .Where(s => s.Warehouse == 1)
                .Where(s=>s.OrderNumber!=null)
                .OrderByDescending(s => s.Id)
                .Select(s => new Serials
                {
                 serialNo = s.SerialNo,
                 makeModel =s.SerialNo+"- "+ s.Make + " " + s.Model,
                 id = s.Id
                 })
                .ToListAsync();

            return Ok(serials);


        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getStock(int id)
        {
            var stock = await _context.DtStocks.FindAsync(id);

            return Ok(stock);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, DtStock updatedStock)
        {
            if (id != updatedStock.Id)
            {
                return BadRequest("Stock ID mismatch.");
            }

            var existingStock = await _context.DtStocks.FindAsync(id);
            if (existingStock == null)
            {
                return NotFound();
            }

            // Manually update each field
        
            existingStock.OrderNumber = updatedStock.OrderNumber;
      
            existingStock.SerialNo = updatedStock.SerialNo;
            existingStock.Make = updatedStock.Make;
            existingStock.Model = updatedStock.Model;
            existingStock.Description = updatedStock.Description;
          
            existingStock.Modified = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool StockExists(int id)
        {
            return _context.DtStocks.Any(e => e.Id == id);
        }


        [HttpGet("stoks")]
        public async Task<ActionResult<IEnumerable<DtStock>>> GetStocks()
        {
            var stocks = await _context.DtStocks
        .Include(s => s.Category)
        .Include(s => s.Project)
        .Include(s => s.Condition)
        .Include(s => s.StockStatus)
        .Include(s => s.Location)
        .Include(s => s.Currency)
        .Where(s => s.Warehouse == 1)
        .Select(s => new
        {
            s.Id,
            s.OrderNumber,
            s.SerialNo,
            s.Make,
            s.Model,
            s.Description,
            Category = s.Category.Category,
            Project = s.Project.Project,
            Condition = s.Condition.Condition,
            StockStatus = s.StockStatus.Status,
            Location = s.Location.Location,
            Currency = s.Currency.Code,
            s.PurchasePrice,
            s.PurchaseDate,
            s.DateReceived
        })
        .OrderByDescending(s => s.Id)
        
        .ToListAsync();

            return Ok(stocks);

            ;
        }
         


        [HttpGet("allstock")]

        public async Task<IActionResult> getAllStock()
        {
            var stocks = await _context.DtStocks.OrderByDescending(s=>s.Id).Select
                (s => new
                {
                    s.Id,
                    s.OrderNumber,
                    s.SerialNo,
                    s.Make,
                    s.Model,
                    s.Itnumber,
                    s.Description,
                    s.CategoryId,
                    s.ProjectId,
                    s.ConditionId,
                    s.StockStatusId,
                    s.LocationId,
                    s.RecordStatusId,
                    s.CurrencyId,
                    s.PurchasePrice,
                    s.Comment,
                    s.PurchaseDate,
                    s.DateReceived,
                    s.CreateDate
                }).
                ToListAsync();
            return Ok(stocks);
        }

        [HttpPost("dispatch")]

        public async Task<IActionResult> dispatchStockFromWarehouse([FromBody]DispatchDto dto)
        {
            if(dto == null)
            {
                return BadRequest("Invalid dispatch data.");
            }

            var stockItem = await _context.DtStocks.FindAsync(dto.StockId);
            if (stockItem == null)
            {
                return NotFound($"Stock item with ID {dto.StockId} not found.");
            }
            stockItem.Warehouse = 2;//dispatch stock from warehouse
            stockItem.LocationId = dto.locationId;
            _context.DtStocks.Update(stockItem);
            await _context.SaveChangesAsync();
            return Ok(new {appdata="Stock Dispatched" });// Assuming 1 means assigned


        }

        [HttpPost("create")]
        public async Task<IActionResult> createStock([FromBody] DtStock stock)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            stock.CreateDate = DateTime.Now;
            stock.Assigned=0;
            stock.Warehouse = 1; 
            Dimension dimension = await GetDimensionByOrderNumber(stock.OrderNumber);
            stock.SiteCode=dimension.SITE_CODE;
            stock.ReqDimension1Code=dimension.REQ_DIMENSION1_CODE;
            stock.ReqDimension2Code= dimension.REQ_DIMENSION2_CODE;
            stock.ReqDimension3Code=dimension.REQ_DIMENSION3_CODE;
            stock.ReqDimension4Code=dimension.REQ_DIMENSION4_CODE;

            _context.DtStocks
                .Add(stock);

            await _context.SaveChangesAsync();

            return Ok(stock);
        }

        [HttpPost("CreateBatch")]
        public async Task<IActionResult> CreateBatch([FromBody] List<CreateStockDto> stocksDto)
        {
            if (stocksDto == null || !stocksDto.Any())
            {
                return BadRequest("No stock data provided.");
            }

            var createdStocks = new List<DtStock>();

            foreach (var dto in stocksDto)
            {
                // Validate required fields
                if (string.IsNullOrEmpty(dto.OrderNumber) ||
                    string.IsNullOrEmpty(dto.SerialNo) ||
                    string.IsNullOrEmpty(dto.Make) ||
                    string.IsNullOrEmpty(dto.Model))
                {
                    return BadRequest("Missing required fields: orderNumber, serialNo, make, and model are required.");
                }

                var stock = new DtStock
                {
                    OrderNumber = dto.OrderNumber,
                    SerialNo = dto.SerialNo,
                    Make = dto.Make,
                    Model = dto.Model,
                    Description = $"{dto.Make} {dto.Model}",
                    CategoryId = dto.CategoryId,
                    ProjectId = dto.ProjectId,
                    ConditionId = dto.ConditionId,
                    CurrencyId = dto.CurrencyId,
                    Comment = dto.Comment,
                    LocationId = dto.LocationId,
                    CreateDate = DateTime.Now,
                    Assigned = 0,
                    Warehouse = 1,
                    StockStatusId = 1, // Available
                    RecordStatusId = 1 // Active
                };

                // Handle price
                if (dto.Price.HasValue)
                {
                    stock.PurchasePrice = dto.Price.Value.ToString("F2");
                }

                // Handle dates
                if (!string.IsNullOrEmpty(dto.PurchaseDate))
                {
                    if (DateOnly.TryParse(dto.PurchaseDate, out DateOnly purchaseDate))
                    {
                        stock.PurchaseDate = purchaseDate;
                    }
                }

                if (!string.IsNullOrEmpty(dto.DateReceived))
                {
                    if (DateOnly.TryParse(dto.DateReceived, out DateOnly dateReceived))
                    {
                        stock.DateReceived = dateReceived;
                    }
                }

                // Get dimension data
                try
                {
                    Dimension dimension = await GetDimensionByOrderNumber(dto.OrderNumber);
                    if (dimension != null)
                    {
                        stock.SiteCode = dimension.SITE_CODE;
                        stock.ReqDimension1Code = dimension.REQ_DIMENSION1_CODE;
                        stock.ReqDimension2Code = dimension.REQ_DIMENSION2_CODE;
                        stock.ReqDimension3Code = dimension.REQ_DIMENSION3_CODE;
                        stock.ReqDimension4Code = dimension.REQ_DIMENSION4_CODE;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue without dimension data
                    Console.WriteLine($"Error fetching dimension for order {dto.OrderNumber}: {ex.Message}");
                }

                _context.DtStocks.Add(stock);
                createdStocks.Add(stock);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving to database: {ex.Message}");
            }

            return Ok(new
            {
                message = $"Successfully created {createdStocks.Count} assets.",
                createdAssets = createdStocks.Select(s => new {
                    s.Id,
                    s.OrderNumber,
                    s.SerialNo,
                    s.Make,
                    s.Model
                })
            });
        }
        private async Task<Dimension?> GetDimensionByOrderNumber(string orderNumber)
        {
            try
            {
                string sql = @"SELECT DISTINCT PORDER.POHNUM_0 AS PO_NUMBER,
                              PORDER.POHFCY_0 AS SITE_CODE,
                              dim.CCE_0 AS REQ_DIMENSION1_CODE,
                              dim.CCE_1 AS REQ_DIMENSION2_CODE,
                              dim.CCE_2 AS REQ_DIMENSION3_CODE,
                              dim.CCE_3 AS REQ_DIMENSION4_CODE,
                              
                              GROPRI_0 AS UNIT_PRICE,
                              '' AS STOCK_CODE,
                              CONCAT(ITMDES1_0, ITMDES_0) AS STOCK_DESC,
                              PORDER.CUR_0 AS STOCK_CAT_CODE,
                              CURDES_0 AS STOCK_CAT_DESC,
                              PORDERP.POPLIN_0
                       FROM [192.168.105.198\SAGESQL].[x3data].[TANDEM].PORDER
                       LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].[PORDERP] ON PORDERP.POHNUM_0 = PORDER.POHNUM_0
                       LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].CPTANALIN dim ON dim.VCRNUM_0 = PORDER.POHNUM_0 AND dim.VCRLIN_0 = PORDERP.POPLIN_0
                       LEFT JOIN [192.168.105.198\SAGESQL].[x3data].[TANDEM].[TABCUR] ON TABCUR.CUR_0 = PORDER.CUR_0
                       WHERE PORDER.POHNUM_0 = @OrderNumber 
                       ORDER BY PORDER.POHNUM_0"
                ;

                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await conn.OpenAsync();

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Dimension
                    {
                        PO_NUMBER = reader["PO_NUMBER"]?.ToString(),
                        SITE_CODE = reader["SITE_CODE"]?.ToString(),
                        REQ_DIMENSION1_CODE = reader["REQ_DIMENSION1_CODE"]?.ToString(),
                        REQ_DIMENSION2_CODE = reader["REQ_DIMENSION2_CODE"]?.ToString(),
                        REQ_DIMENSION3_CODE = reader["REQ_DIMENSION3_CODE"]?.ToString(),
                        REQ_DIMENSION4_CODE = reader["REQ_DIMENSION4_CODE"]?.ToString(),

                    };
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
               
            }
            return null;
        }

        [HttpPost("assign")]
        [HttpPost]
        public async Task<IActionResult> AssignItem([FromBody] AppAssignment assignment)
        {
            try
            {
                if (assignment == null)
                {
                    return BadRequest("Invalid assignment data.");
                }

                // Set metadata
                assignment.CreatedDate = DateTime.Now;
                assignment.DateAssigned = DateTime.Now;
                assignment.LastUpdateDate = DateTime.Now;
                assignment.RecordStatusId = 7;
                assignment.Status = 1; // Assigned

                // Check if the stock item exists and include Category
                DtStock stockItem = await _context.DtStocks
                    .Include(s => s.Category)
                    .FirstOrDefaultAsync(s => s.Id == assignment.StockId);

                if (stockItem == null)
                {
                    return NotFound($"Stock item with ID {assignment.StockId} not found.");
                }

                if (stockItem.Category == null)
                {
                    return BadRequest("The stock item does not have a category assigned.");
                }

                stockItem.Assigned = 1;

                // Check if the employee exists
                DtEmployee e = await _context.DtEmployees.FindAsync(assignment.EmployeeId);
                if (e == null)
                {
                    return NotFound($"Employee with ID {assignment.EmployeeId} not found.");
                }

                if (string.IsNullOrEmpty(e.Email))
                {
                    return BadRequest("Employee does not have an email address on record.");
                }

                // Add the assignment
                _context.AppAssignments.Add(assignment);

                // Update the stock item
                _context.DtStocks.Update(stockItem);

                // Prepare email body
                string emailBody = $@"
<p>You have been assigned a new item:</p>
<ul>
    <li><strong>Type:</strong> {stockItem.Category.Category}</li>
    <li><strong>Make:</strong> {stockItem.Make ?? "N/A"}</li>
    <li><strong>Model:</strong> {stockItem.Model ?? "N/A"}</li>
    <li><strong>Serial:</strong> {stockItem.SerialNo ?? "N/A"}</li>
</ul>";

                // Send email
               // await _emailService.SendEmailAsync(e.Email, "New Item Assigned", emailBody);

                // Save changes to DB
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAllStocks), new { id = assignment.Id }, assignment);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
               
            }
        }


        [HttpPost("borrow")]

        public async Task<IActionResult> BorrowItem([FromBody] AppBorrow borrow)
        {

            if(borrow == null)
            {
                return BadRequest("Invalid borrow Data");
            }
            borrow.CreatedDate = DateTime.Now;
            borrow.Status = 1;
            DtStock stockItem = await _context.DtStocks.FindAsync(borrow.StockId);
            if (stockItem == null)
            {
                return NotFound($"Stock item with ID {borrow.StockId} not found.");
            }
            stockItem.Assigned = 2;//2 will denote a borrowed Item
            
            _context.AppBorrows.Add(borrow);
            _context.DtStocks.Update(stockItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllStocks), new { id = borrow.Id }, borrow);

        }

        [HttpPost("maintain")]
        public async Task<IActionResult> MaintainItem([FromBody] AppMaitenance maintenance)
        {
            if (maintenance == null) // Corrected variable name
            {
                return BadRequest("Invalid maintenance data."); // Updated error message
            }
            maintenance.CreatedDate = DateTime.Now; // Corrected variable name
            maintenance.CheckInDate = DateTime.Now;
            maintenance.Status = 1;//It has not been taken out of maintenance
            // Check if the stock item exists
            var stockItem = await _context.DtStocks.FindAsync(maintenance.StockId); // Corrected variable name
            if (stockItem == null)
            {
                return NotFound($"Stock item with ID {maintenance.StockId} not found."); // Corrected variable name
            }

            // Add the maintenance record to the database
            _context.AppMaitenances.Add(maintenance); // Corrected variable name
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAllStocks), new { id = maintenance.Id }, maintenance); // Corrected variable name
        }
    }
}


public class CreateStockDto
{
    public string? OrderNumber { get; set; }
    public string? SerialNo { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? CategoryId { get; set; }
    public int? ProjectId { get; set; }
    public int? ConditionId { get; set; }
    public int? CurrencyId { get; set; }
    public decimal? Price { get; set; }
    public string? PurchaseDate { get; set; }
    public string? DateReceived { get; set; }
    public string? Comment { get; set; }
    public int? LocationId { get; set; }
}