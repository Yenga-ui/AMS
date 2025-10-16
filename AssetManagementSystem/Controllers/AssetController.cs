using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AssetController : ControllerBase
{
    private readonly AssetContext _context;

    public AssetController(AssetContext context)
    {
        _context = context;
    }

    // GET: api/Asset/available - Get available assets for assignment/borrow
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<object>>> GetAvailableAssets()
    {
        var assets = await _context.DtStocks
            .Where(s => s.Warehouse == 2 && s.Assigned == 0) // Available in warehouse, not assigned, active status
            .Select(s => new
            {
                s.Id,
                MakeModel = s.Make + " " + s.Model + " (" + s.SerialNo + ")",
                s.SerialNo,
                s.Make,
                s.Model,
                s.Itnumber,
                Category = s.Category.Category,
                Condition = s.Condition.Condition
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
            .Include(a => a.Stock)
            .Include(a => a.Employee)
            .Include(a => a.Location)
            .Include(a => a.Project)
            .Where(a => a.Status == 1) // Active assignments
            .Select(a => new
            {
                a.Id,
                AssetId = a.StockId,
                Asset = a.Stock.Make + " " + a.Stock.Model + " (" + a.Stock.SerialNo + ")",
                Employee = a.Employee.Fullname,
                EmployeeId = a.EmployeeId,
                Location = a.Location.Location,
                Project = a.Project.Project,
                a.DateAssigned,
                a.Comments
            })
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
        {
            query = query.Where(a =>
                a.Asset.Contains(paginationParams.SearchTerm) ||
                a.Employee.Contains(paginationParams.SearchTerm) ||
                a.Location.Contains(paginationParams.SearchTerm));
        }

        // Get total count before pagination
        var totalRecords = await query.CountAsync();

        // Apply pagination
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
            .Include(b => b.Stock)
            .Include(b => b.Employee)
            .Where(b => b.Status == 1) // Active borrows
            .Select(b => new
            {
                b.Id,
                AssetId = b.StockId,
                Asset = b.Stock.Make + " " + b.Stock.Model + " (" + b.Stock.SerialNo + ")",
                Employee = b.Employee.Fullname,
                EmployeeId = b.EmployeeId,
                b.DateFrom,
                b.DateTo,
                b.CreatedDate,
                DaysRemaining = EF.Functions.DateDiffDay(DateTime.Now, b.DateTo)
            })
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
        {
            query = query.Where(b =>
                b.Asset.Contains(paginationParams.SearchTerm) ||
                b.Employee.Contains(paginationParams.SearchTerm));
        }

        // Get total count before pagination
        var totalRecords = await query.CountAsync();

        // Apply pagination
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
            .Include(m => m.Stock)
            .Include(m => m.Employee)
            .Where(m => m.Status == 1) // Active maintenance
            .Select(m => new
            {
                m.Id,
                AssetId = m.StockId,
                Asset = m.Stock.Make + " " + m.Stock.Model + " (" + m.Stock.SerialNo + ")",
                Employee = m.Employee.Fullname,
                EmployeeId = m.EmployeeId,
                m.Ticket,
                m.Diagnosis,
                m.CheckInDate,
                m.ReturnDate,
                DaysUntilReturn = EF.Functions.DateDiffDay(DateTime.Now, m.ReturnDate)
            })
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
        {
            query = query.Where(m =>
                m.Asset.Contains(paginationParams.SearchTerm) ||
                m.Employee.Contains(paginationParams.SearchTerm) ||
                m.Ticket.Contains(paginationParams.SearchTerm));
        }

        // Get total count before pagination
        var totalRecords = await query.CountAsync();

        // Apply pagination
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
            // Check if asset exists and is available
            var asset = await _context.DtStocks.FindAsync(assignDto.StockId);
            if (asset == null)
            {
                return BadRequest("Asset not found");
            }

            if (asset.Assigned == 1)
            {
                return BadRequest("Asset is already assigned");
            }

            // Check if employee exists
            var employee = await _context.DtEmployees.FindAsync(assignDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            // Create assignment record
            var assignment = new AppAssignment
            {
                StockId = assignDto.StockId,
                EmployeeId = assignDto.EmployeeId,
                LocationId = assignDto.LocationId,
                ProjectId = assignDto.ProjectId,
                Comments = assignDto.Comments,
                DateAssigned = DateTime.Now,
                CreatedDate = DateTime.Now,
                Status = 1 // Active
            };

            // Update asset status
            asset.Assigned = 1;
            asset.StockStatusId = 2; // Assigned status

            _context.AppAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Return assignment details
            var result = await _context.AppAssignments
                .Include(a => a.Stock)
                .Include(a => a.Employee)
                .Include(a => a.Location)
                .Include(a => a.Project)
                .Where(a => a.Id == assignment.Id)
                .Select(a => new
                {
                    a.Id,
                    Asset = a.Stock.Make + " " + a.Stock.Model + " (" + a.Stock.SerialNo + ")",
                    Employee = a.Employee.Fullname,
                    Location = a.Location.Location,
                    Project = a.Project.Project,
                    a.DateAssigned,
                    a.Comments
                })
                .FirstOrDefaultAsync();

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
            // Check if asset exists and is available
            var asset = await _context.DtStocks.FindAsync(borrowDto.StockId);
            if (asset == null)
            {
                return BadRequest("Asset not found");
            }

            if (asset.Assigned == 1)
            {
                return BadRequest("Asset is already assigned or borrowed");
            }

            // Check if employee exists
            var employee = await _context.DtEmployees.FindAsync(borrowDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            // Check if return date is after borrow date
            if (borrowDto.DateTo <= borrowDto.DateFrom)
            {
                return BadRequest("Return date must be after borrow date");
            }

            // Create borrow record
            var borrow = new AppBorrow
            {
                StockId = borrowDto.StockId,
                EmployeeId = borrowDto.EmployeeId,
                DateFrom = borrowDto.DateFrom,
                DateTo = borrowDto.DateTo,
                CreatedDate = DateTime.Now,
                Status = 1 // Active
            };

            // Update asset status
            asset.Assigned = 1;
            asset.StockStatusId = 3; // Borrowed status

            _context.AppBorrows.Add(borrow);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Return borrow details
            var result = await _context.AppBorrows
                .Include(b => b.Stock)
                .Include(b => b.Employee)
                .Where(b => b.Id == borrow.Id)
                .Select(b => new
                {
                    b.Id,
                    Asset = b.Stock.Make + " " + b.Stock.Model + " (" + b.Stock.SerialNo + ")",
                    Employee = b.Employee.Fullname,
                    b.DateFrom,
                    b.DateTo,
                    b.CreatedDate
                })
                .FirstOrDefaultAsync();

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
            // Check if asset exists
            var asset = await _context.DtStocks.FindAsync(maintainDto.StockId);
            if (asset == null)
            {
                return BadRequest("Asset not found");
            }

            // Check if employee exists
            var employee = await _context.DtEmployees.FindAsync(maintainDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            // Create maintenance record
            var maintenance = new AppMaitenance
            {
                StockId = maintainDto.StockId,
                EmployeeId = maintainDto.EmployeeId,
                Ticket = maintainDto.Ticket,
                Diagnosis = maintainDto.Diagnosis,
                ReturnDate = maintainDto.ReturnDate,
                CheckInDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                Status = 1 // In progress
            };

            // Update asset status
            asset.StockStatusId = 4; // Under maintenance status

            _context.AppMaitenances.Add(maintenance);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Return maintenance details
            var result = await _context.AppMaitenances
                .Include(m => m.Stock)
                .Include(m => m.Employee)
                .Where(m => m.Id == maintenance.Id)
                .Select(m => new
                {
                    m.Id,
                    Asset = m.Stock.Make + " " + m.Stock.Model + " (" + m.Stock.SerialNo + ")",
                    Employee = m.Employee.Fullname,
                    m.Ticket,
                    m.Diagnosis,
                    m.CheckInDate,
                    m.ReturnDate
                })
                .FirstOrDefaultAsync();

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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var asset = await _context.DtStocks.FindAsync(returnDto.StockId);
            if (asset == null)
            {
                return BadRequest("Asset not found");
            }

            // Check if asset is assigned
            var assignment = await _context.AppAssignments
                .FirstOrDefaultAsync(a => a.StockId == returnDto.StockId && a.Status == 1);

            // Check if asset is borrowed
            var borrow = await _context.AppBorrows
                .FirstOrDefaultAsync(b => b.StockId == returnDto.StockId && b.Status == 1);

            if (assignment == null && borrow == null)
            {
                return BadRequest("Asset is not currently assigned or borrowed");
            }

            // Update assignment status if exists
            if (assignment != null)
            {
                assignment.Status = 0; // Inactive
                assignment.LastUpdateDate = DateTime.Now;
            }

            // Update borrow status if exists
            if (borrow != null)
            {
                borrow.Status = 0; // Inactive
            }

            // Update asset status
            asset.Assigned = 0;
            asset.StockStatusId = 1; // Available status

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
}

// DTOs for asset operations
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