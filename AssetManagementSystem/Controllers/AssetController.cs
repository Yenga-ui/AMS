using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagementSystem.Models;
using AssetManagementSystem.Services; // for IEmailService

namespace AssetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly AssetContext _context;
        private readonly IEmailService _emailService;

        public AssetController(AssetContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: api/Asset/available - Get available assets for assignment/borrow
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableAssets()
        {
            var assets = await _context.DtStocks
                .Where(s => s.Warehouse == 2 && s.Assigned == 0)
                .Select(s => new
                {
                    s.Id,
                    MakeModel = s.Make + " " + s.Model + " (" + s.SerialNo + ")",
                    s.SerialNo,
                    s.Make,
                    s.Model,
                    s.Itnumber,
                    Category = s.Category != null ? s.Category.Category : string.Empty,
                    Condition = s.Condition != null ? s.Condition.Condition : string.Empty
                })
                .ToListAsync();

            return Ok(assets);
        }

        // GET: api/Asset/assigned - Get assigned assets
        [HttpGet("assigned")]
        public async Task<ActionResult<PagedResponse<object>>> GetAssignedAssets(
            [FromQuery] PaginationParams paginationParams)
        {
            var query = _context.AppAssignments
                .Include(a => a.Stock).ThenInclude(s => s.Category)
                .Include(a => a.Employee)
                .Include(a => a.Location)
                .Include(a => a.Project)
                .Where(a => a.Status == 1)
                .Select(a => new
                {
                    a.Id,
                    AssetId = a.StockId,
                    Asset = a.Stock.Make + " " + a.Stock.Model + " (" + a.Stock.SerialNo + ")",
                    a.Stock.Make,
                    a.Stock.Model,
                    SerialNo = a.Stock.SerialNo,
                    Type = a.Stock.Category != null ? a.Stock.Category.Category : string.Empty,
                    Employee = a.Employee.Fullname,
                    EmployeeId = a.EmployeeId,
                    Location = a.Location != null ? a.Location.Location : string.Empty,
                    Project = a.Project != null ? a.Project.Project : string.Empty,
                    a.DateAssigned,
                    a.Comments
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
            {
                query = query.Where(a =>
                    a.Asset.Contains(paginationParams.SearchTerm) ||
                    a.Employee.Contains(paginationParams.SearchTerm) ||
                    a.Location.Contains(paginationParams.SearchTerm));
            }

            var totalRecords = await query.CountAsync();

            var assignedAssets = await query
                .OrderByDescending(a => a.DateAssigned)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<object>(
                assignedAssets, paginationParams.PageNumber, paginationParams.PageSize, totalRecords);

            return Ok(pagedResponse);
        }

        // GET: api/Asset/borrowed - Get borrowed assets
        [HttpGet("borrowed")]
        public async Task<ActionResult<PagedResponse<object>>> GetBorrowedAssets(
            [FromQuery] PaginationParams paginationParams)
        {
            var query = _context.AppBorrows
                .Include(b => b.Stock).ThenInclude(s => s.Category)
                .Include(b => b.Employee)
                .Where(b => b.Status == 1)
                .Select(b => new
                {
                    b.Id,
                    AssetId = b.StockId,
                    Asset = b.Stock.Make + " " + b.Stock.Model + " (" + b.Stock.SerialNo + ")",
                    b.Stock.Make,
                    b.Stock.Model,
                    SerialNo = b.Stock.SerialNo,
                    Type = b.Stock.Category != null ? b.Stock.Category.Category : string.Empty,
                    Employee = b.Employee.Fullname,
                    EmployeeId = b.EmployeeId,
                    b.DateFrom,
                    b.DateTo,
                    b.CreatedDate,
                    DaysRemaining = EF.Functions.DateDiffDay(DateTime.Now, b.DateTo)
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
            {
                query = query.Where(b =>
                    b.Asset.Contains(paginationParams.SearchTerm) ||
                    b.Employee.Contains(paginationParams.SearchTerm));
            }

            var totalRecords = await query.CountAsync();

            var borrowedAssets = await query
                .OrderBy(b => b.DaysRemaining)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<object>(
                borrowedAssets, paginationParams.PageNumber, paginationParams.PageSize, totalRecords);

            return Ok(pagedResponse);
        }

        // GET: api/Asset/maintenance - Get maintenance requests
        [HttpGet("maintenance")]
        public async Task<ActionResult<PagedResponse<object>>> GetMaintenanceRequests(
            [FromQuery] PaginationParams paginationParams)
        {
            var query = _context.AppMaitenances
                .Include(m => m.Stock).ThenInclude(s => s.Category)
                .Include(m => m.Employee)
                .Where(m => m.Status == 1)
                .Select(m => new
                {
                    m.Id,
                    AssetId = m.StockId,
                    Asset = m.Stock.Make + " " + m.Stock.Model + " (" + m.Stock.SerialNo + ")",
                    m.Stock.Make,
                    m.Stock.Model,
                    SerialNo = m.Stock.SerialNo,
                    Type = m.Stock.Category != null ? m.Stock.Category.Category : string.Empty,
                    Employee = m.Employee.Fullname,
                    EmployeeId = m.EmployeeId,
                    m.Ticket,
                    m.Diagnosis,
                    m.CheckInDate,
                    m.ReturnDate,
                    DaysUntilReturn = EF.Functions.DateDiffDay(DateTime.Now, m.ReturnDate)
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
            {
                query = query.Where(m =>
                    m.Asset.Contains(paginationParams.SearchTerm) ||
                    m.Employee.Contains(paginationParams.SearchTerm) ||
                    m.Ticket.Contains(paginationParams.SearchTerm));
            }

            var totalRecords = await query.CountAsync();

            var maintenanceRequests = await query
                .OrderBy(m => m.DaysUntilReturn)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<object>(
                maintenanceRequests, paginationParams.PageNumber, paginationParams.PageSize, totalRecords);

            return Ok(pagedResponse);
        }

        // POST: api/Asset/assign - Assign asset to employee
        [HttpPost("assign")]
        public async Task<IActionResult> AssignAsset(AssignAssetDto assignDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var asset = await _context.DtStocks.FindAsync(assignDto.StockId);
                if (asset == null) return BadRequest("Asset not found");
                if (asset.Assigned == 1) return BadRequest("Asset is already assigned");

                var employee = await _context.DtEmployees.FindAsync(assignDto.EmployeeId);
                if (employee == null) return BadRequest("Employee not found");

                var assignment = new AppAssignment
                {
                    StockId = assignDto.StockId,
                    EmployeeId = assignDto.EmployeeId,
                    LocationId = assignDto.LocationId,
                    ProjectId = assignDto.ProjectId,
                    Comments = assignDto.Comments,
                    DateAssigned = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    Status = 1
                };

                asset.Assigned = 1;
                asset.StockStatusId = 2; // Assigned

                _context.AppAssignments.Add(assignment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Build result with fields needed for response and email
                var result = await _context.AppAssignments
                    .Include(a => a.Stock).ThenInclude(s => s.Category)
                    .Include(a => a.Employee)
                    .Include(a => a.Location)
                    .Include(a => a.Project)
                    .Where(a => a.Id == assignment.Id)
                    .Select(a => new
                    {
                        a.Id,
                        Asset = a.Stock.Make + " " + a.Stock.Model + " (" + a.Stock.SerialNo + ")",
                        Type = a.Stock.Category != null ? a.Stock.Category.Category : string.Empty,
                        Make = a.Stock.Make,
                        Model = a.Stock.Model,
                        SerialNo = a.Stock.SerialNo,
                        Employee = a.Employee.Fullname,
                        EmployeeEmail = a.Employee.Email,
                        Location = a.Location != null ? a.Location.Location : string.Empty,
                        Project = a.Project != null ? a.Project.Project : string.Empty,
                        a.DateAssigned,
                        a.Comments
                    })
                    .FirstOrDefaultAsync();

                // send email — swallow but log exceptions so main flow succeeds
                _ = SendAssignmentEmailAsync(result).ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Console.WriteLine("[EMAIL ERROR] " + t.Exception.Flatten().Message);
                    }
                });

                return Ok(new { message = "Asset assigned successfully", data = result });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Asset/borrow - Borrow asset to employee
        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowAsset(BorrowAssetDto borrowDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var asset = await _context.DtStocks.FindAsync(borrowDto.StockId);
                if (asset == null) return BadRequest("Asset not found");
                if (asset.Assigned == 1) return BadRequest("Asset is already assigned or borrowed");

                var employee = await _context.DtEmployees.FindAsync(borrowDto.EmployeeId);
                if (employee == null) return BadRequest("Employee not found");

                if (borrowDto.DateTo <= borrowDto.DateFrom) return BadRequest("Return date must be after borrow date");

                var borrow = new AppBorrow
                {
                    StockId = borrowDto.StockId,
                    EmployeeId = borrowDto.EmployeeId,
                    DateFrom = borrowDto.DateFrom,
                    DateTo = borrowDto.DateTo,
                    CreatedDate = DateTime.Now,
                    Status = 1
                };

                asset.Assigned = 1;
                asset.StockStatusId = 3; // Borrowed

                _context.AppBorrows.Add(borrow);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = await _context.AppBorrows
                    .Include(b => b.Stock).ThenInclude(s => s.Category)
                    .Include(b => b.Employee)
                    .Where(b => b.Id == borrow.Id)
                    .Select(b => new
                    {
                        b.Id,
                        Asset = b.Stock.Make + " " + b.Stock.Model + " (" + b.Stock.SerialNo + ")",
                        Type = b.Stock.Category != null ? b.Stock.Category.Category : string.Empty,
                        Make = b.Stock.Make,
                        Model = b.Stock.Model,
                        SerialNo = b.Stock.SerialNo,
                        Employee = b.Employee.Fullname,
                        EmployeeEmail = b.Employee.Email,
                        b.DateFrom,
                        b.DateTo,
                        b.CreatedDate
                    })
                    .FirstOrDefaultAsync();

                _ = SendBorrowEmailAsync(result).ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Console.WriteLine("[EMAIL ERROR] " + t.Exception.Flatten().Message);
                    }
                });

                return Ok(new { message = "Asset borrowed successfully", data = result });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Asset/maintain - Submit asset for maintenance
        [HttpPost("maintain")]
        public async Task<IActionResult> MaintainAsset(MaintainAssetDto maintainDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var asset = await _context.DtStocks.FindAsync(maintainDto.StockId);
                if (asset == null) return BadRequest("Asset not found");

                var employee = await _context.DtEmployees.FindAsync(maintainDto.EmployeeId);
                if (employee == null) return BadRequest("Employee not found");

                var maintenance = new AppMaitenance
                {
                    StockId = maintainDto.StockId,
                    EmployeeId = maintainDto.EmployeeId,
                    Ticket = maintainDto.Ticket,
                    Diagnosis = maintainDto.Diagnosis,
                    ReturnDate = maintainDto.ReturnDate,
                    CheckInDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    Status = 1
                };

                asset.StockStatusId = 4; // Under maintenance

                _context.AppMaitenances.Add(maintenance);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = await _context.AppMaitenances
                    .Include(m => m.Stock).ThenInclude(s => s.Category)
                    .Include(m => m.Employee)
                    .Where(m => m.Id == maintenance.Id)
                    .Select(m => new
                    {
                        m.Id,
                        Asset = m.Stock.Make + " " + m.Stock.Model + " (" + m.Stock.SerialNo + ")",
                        Type = m.Stock.Category != null ? m.Stock.Category.Category : string.Empty,
                        Make = m.Stock.Make,
                        Model = m.Stock.Model,
                        SerialNo = m.Stock.SerialNo,
                        Employee = m.Employee.Fullname,
                        EmployeeEmail = m.Employee.Email,
                        m.Ticket,
                        m.Diagnosis,
                        m.CheckInDate,
                        m.ReturnDate
                    })
                    .FirstOrDefaultAsync();

                _ = SendMaintenanceEmailAsync(result).ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Console.WriteLine("[EMAIL ERROR] " + t.Exception.Flatten().Message);
                    }
                });

                return Ok(new { message = "Maintenance request submitted successfully", data = result });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Asset/return - Return an asset (from assignment or borrow)
        [HttpPost("return")]
        public async Task<IActionResult> ReturnAsset(ReturnAssetDto returnDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var asset = await _context.DtStocks.FindAsync(returnDto.StockId);
                if (asset == null) return BadRequest("Asset not found");

                var assignment = await _context.AppAssignments
                    .FirstOrDefaultAsync(a => a.StockId == returnDto.StockId && a.Status == 1);

                var borrow = await _context.AppBorrows
                    .FirstOrDefaultAsync(b => b.StockId == returnDto.StockId && b.Status == 1);

                if (assignment == null && borrow == null)
                {
                    return BadRequest("Asset is not currently assigned or borrowed");
                }

                if (assignment != null)
                {
                    assignment.Status = 0;
                    assignment.LastUpdateDate = DateTime.Now;
                }

                if (borrow != null)
                {
                    borrow.Status = 0;
                }

                asset.Assigned = 0;
                asset.StockStatusId = 1; // Available

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Asset returned successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Asset/status - Get asset status summary
        [HttpGet("status")]
        public async Task<ActionResult<object>> GetAssetStatusSummary()
        {
            var totalAssets = await _context.DtStocks.CountAsync();
            var assignedAssets = await _context.AppAssignments.CountAsync(a => a.Status == 1);
            var borrowedAssets = await _context.AppBorrows.CountAsync(b => b.Status == 1);
            var maintenanceAssets = await _context.AppMaitenances.CountAsync(m => m.Status == 1);
            var availableAssets = totalAssets - assignedAssets - borrowedAssets - maintenanceAssets;

            return Ok(new
            {
                TotalAssets = totalAssets,
                AvailableAssets = availableAssets,
                AssignedAssets = assignedAssets,
                BorrowedAssets = borrowedAssets,
                MaintenanceAssets = maintenanceAssets
            });
        }

        #region Email helper methods

        private string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            if (hour >= 5 && hour < 12) return "Good morning";
            if (hour >= 12 && hour < 17) return "Good afternoon";
            return "Good evening";
        }

        private string FormatDateLong(DateTime dt)
        {
            // Example: "Thursday, October 16, 2025 4:32:23 PM"
            return dt.ToString("dddd, MMMM d, yyyy h:mm:ss tt");
        }

        private async Task SendAssignmentEmailAsync(dynamic result)
        {
            try
            {
                if (result == null || string.IsNullOrEmpty(result.EmployeeEmail)) return;

                var greeting = GetGreeting();
                var employeeName = result.Employee ?? "User";
                var dateAssigned = result.DateAssigned != null ? FormatDateLong(result.DateAssigned) : FormatDateLong(DateTime.Now);
                var type = result.Type ?? string.Empty;
                var make = result.Make ?? string.Empty;
                var model = result.Model ?? string.Empty;
                var serial = result.SerialNo ?? string.Empty;
                var location = result.Location ?? string.Empty;
                var remarks = result.Comments ?? string.Empty;

                var bodyHtml = $@"
<p>{greeting}, {employeeName}</p>

<p>You have been assigned an item on {dateAssigned} with the following properties:</p>

<ul>
    <li><strong>Type:</strong> {type}</li>
    <li><strong>Make:</strong> {make}</li>
    <li><strong>Model:</strong> {model}</li>
    <li><strong>Serial number:</strong> {serial}</li>
</ul>

<p>Your details are as follows:<br/>
<strong>Location:</strong> {location}</p>

<p><strong>Remarks:</strong><br/>{remarks}</p>

<p>Regards<br/><br/>CIDRZ ICT</p>
";

                await _emailService.SendEmailAsync(result.EmployeeEmail, "Asset Assignment Notification", bodyHtml);
                Console.WriteLine($"[SUCCESS] Assignment email sent to {result.EmployeeEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Assign: {ex.Message}");
            }
        }

        private async Task SendBorrowEmailAsync(dynamic result)
        {
            try
            {
                if (result == null || string.IsNullOrEmpty(result.EmployeeEmail)) return;

                var greeting = GetGreeting();
                var employeeName = result.Employee ?? "User";
                var dateAssigned = result.CreatedDate != null ? FormatDateLong(result.CreatedDate) : FormatDateLong(DateTime.Now);
                var type = result.Type ?? string.Empty;
                var make = result.Make ?? string.Empty;
                var model = result.Model ?? string.Empty;
                var serial = result.SerialNo ?? string.Empty;
                var dateFrom = result.DateFrom != null ? FormatDateLong(result.DateFrom) : string.Empty;
                var dateTo = result.DateTo != null ? FormatDateLong(result.DateTo) : string.Empty;

                var bodyHtml = $@"
<p>{greeting}, {employeeName}</p>

<p>You have been assigned an item on {dateAssigned} with the following properties:</p>

<ul>
    <li><strong>Type:</strong> {type}</li>
    <li><strong>Make:</strong> {make}</li>
    <li><strong>Model:</strong> {model}</li>
    <li><strong>Serial number:</strong> {serial}</li>
</ul>

<p>Your details are as follows:<br/>
<strong>Borrowed From:</strong> {dateFrom}<br/>
<strong>Borrowed To:</strong> {dateTo}</p>

<p>Remarks:<br/>Please ensure the asset is returned on or before the due date.</p>

<p>Regards<br/><br/>CIDRZ ICT</p>
";

                await _emailService.SendEmailAsync(result.EmployeeEmail, "Asset Borrow Notification", bodyHtml);
                Console.WriteLine($"[SUCCESS] Borrow email sent to {result.EmployeeEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Borrow: {ex.Message}");
            }
        }

        private async Task SendMaintenanceEmailAsync(dynamic result)
        {
            try
            {
                if (result == null || string.IsNullOrEmpty(result.EmployeeEmail)) return;

                var greeting = GetGreeting();
                var employeeName = result.Employee ?? "User";
                var dateAssigned = result.CheckInDate != null ? FormatDateLong(result.CheckInDate) : FormatDateLong(DateTime.Now);
                var type = result.Type ?? string.Empty;
                var make = result.Make ?? string.Empty;
                var model = result.Model ?? string.Empty;
                var serial = result.SerialNo ?? string.Empty;
                var ticket = result.Ticket ?? string.Empty;
                var diagnosis = result.Diagnosis ?? string.Empty;
                var returnDate = result.ReturnDate != null ? FormatDateLong(result.ReturnDate) : string.Empty;

                var bodyHtml = $@"
<p>{greeting}, {employeeName}</p>

<p>You have been assigned an item on {dateAssigned} with the following properties:</p>

<ul>
    <li><strong>Type:</strong> {type}</li>
    <li><strong>Make:</strong> {make}</li>
    <li><strong>Model:</strong> {model}</li>
    <li><strong>Serial number:</strong> {serial}</li>
</ul>

<p>Your details are as follows:<br/>
<strong>Ticket:</strong> {ticket}<br/>
<strong>Expected Return:</strong> {returnDate}</p>

<p><strong>Remarks / Diagnosis:</strong><br/>{diagnosis}</p>

<p>Regards<br/><br/>CIDRZ ICT</p>
";

                await _emailService.SendEmailAsync(result.EmployeeEmail, "Asset Maintenance Ticket Acknowledgement", bodyHtml);
                Console.WriteLine($"[SUCCESS] Maintenance email sent to {result.EmployeeEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Maintenance: {ex.Message}");
            }
        }

        #endregion

    } // end controller

    // DTOs for asset operations (kept here for single-file completeness)
    public class AssignAssetDto
    {
        public int StockId { get; set; }
        public int EmployeeId { get; set; }
        public int LocationId { get; set; }
        public int ProjectId { get; set; }
        public string Comments { get; set; }
    }

    public class AssignedAssetDto
    {
        public int Id { get; set; }
        public int? AssetId { get; set; }
        public string Asset { get; set; }
        public string Employee { get; set; }
        public int? EmployeeId { get; set; }
        public string Location { get; set; }
        public string Project { get; set; }
        public DateTime? DateAssigned { get; set; }
        public string Comments { get; set; }
    }

    public class BorrowAssetDto
    {
        public int StockId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public class MaintainAssetDto
    {
        public string Ticket { get; set; }
        public int StockId { get; set; }
        public int EmployeeId { get; set; }
        public string Diagnosis { get; set; }
        public DateTime ReturnDate { get; set; }
    }

    public class ReturnAssetDto
    {
        public int StockId { get; set; }
        public string Comments { get; set; }
    }

    // Simple pagination classes used earlier
    public class PaginationParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
    }

    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
        }
    }
}
