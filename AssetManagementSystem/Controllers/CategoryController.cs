using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly AssetContext _context;

    public CategoryController(AssetContext context)
    {
        _context = context;
    }

    // GET: api/Category/categories
    [HttpGet("categories")]
    public async Task<ActionResult<PagedResponse<CategoryResponseDto>>> GetCategories(
      [FromQuery] PaginationParams paginationParams)
    {
        try
        {
            // 1️⃣ Base query with left join, project to anonymous type
            var baseQuery = from category in _context.DtCategories
                            join type in _context.DtCategoryTypes
                                on category.TypeId equals type.Id into typeJoin
                            from type in typeJoin.DefaultIfEmpty()
                            select new
                            {
                                category.Id,
                                category.Category,
                                category.Description,
                                TypeName = type != null ? type.Type : null,
                               // category.TypeId,
                                category.CreatedDate,
                                category.LastUpdated,
                                category.RecordStatusId
                            };

            var query = baseQuery.AsQueryable();

            // 2️⃣ Apply search filter if provided
            if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
            {
                string search = paginationParams.SearchTerm.Trim();
                query = query.Where(c =>
                    c.Category.Contains(search) ||
                    (c.Description != null && c.Description.Contains(search)) ||
                    (c.TypeName != null && c.TypeName.Contains(search))
                );
            }

            // 3️⃣ Get total records before pagination
            var totalRecords = await query.CountAsync();

            // 4️⃣ Apply pagination
            var categoriesRaw = await query
                .OrderBy(c => c.Category)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            // 5️⃣ Map to DTO in memory
            var categories = categoriesRaw.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Category = c.Category,
                Description = c.Description,
                Type = c.TypeName,
                //TypeId = c.TypeId,
                CreatedDate = c.CreatedDate,
                LastUpdated = c.LastUpdated,
                RecordStatusId = c.RecordStatusId
            }).ToList();

            // 6️⃣ Return paged response
            var pagedResponse = new PagedResponse<CategoryResponseDto>(
                categories, paginationParams.PageNumber, paginationParams.PageSize, totalRecords);

            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal Server Error: " + ex.Message });
        }
    }


    // GET: api/Category/categories/5
    [HttpGet("categories/{id}")]
    public async Task<ActionResult<object>> GetCategory(int id)
    {
        var category = await (from c in _context.DtCategories
                              join t in _context.DtCategoryTypes on c.TypeId equals t.Id into typeJoin
                              from t in typeJoin.DefaultIfEmpty()
                              where c.Id == id
                              select new
                              {
                                  c.Id,
                                  c.Category,
                                  c.Description,
                                  Type = t.Type,
                                  TypeId = c.TypeId,
                                  c.CreatedDate,
                                  c.LastUpdated,
                                  c.RecordStatusId
                              }).FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }

        return category;
    }

    // GET: api/Category/types
    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<object>>> GetCategoryTypes()
    {
        var types = await _context.DtCategoryTypes
            .Select(t => new
            {
                t.Id,
                t.Type,
                t.Description
            })
            .ToListAsync();

        return Ok(types);
    }

    // POST: api/Category/categories
    [HttpPost("categories")]
    public async Task<ActionResult<object>> CreateCategory(CategoryCreateDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if category name already exists
        if (await _context.DtCategories.AnyAsync(c => c.Category == categoryDto.Category))
        {
            return BadRequest("Category name already exists");
        }

        var category = new DtCategory
        {
            Category = categoryDto.Category,
            Description = categoryDto.Description,
            TypeId = Int32.Parse(categoryDto.TypeId),
            CreatedDate = DateTime.Now,
            RecordStatusId = 7 // Active status
        };

        _context.DtCategories.Add(category);
        await _context.SaveChangesAsync();

        // Return the created category with type name
        var createdCategory = await (from c in _context.DtCategories
                                     join t in _context.DtCategoryTypes on c.TypeId equals t.Id into typeJoin
                                     from t in typeJoin.DefaultIfEmpty()
                                     where c.Id == category.Id
                                     select new
                                     {
                                         c.Id,
                                         c.Category,
                                         c.Description,
                                         Type = t.Type,
                                         TypeId = c.TypeId,
                                         c.CreatedDate
                                     }).FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, createdCategory);
    }

    // GET: api/Category/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<DtCategory>>> GetAllCategories()
    {
        var categories = await _context.DtCategories
            .OrderBy(c => c.Category)
            .Select(c => new
            {
                c.Id,
                c.Category
            })
            .ToListAsync();

        return Ok(categories);
    }

    // PUT: api/Category/categories/5
    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto categoryDto)
    {
        if (id != categoryDto.Id)
        {
            return BadRequest();
        }

        var category = await _context.DtCategories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        // Check if category name already exists (excluding current category)
        if (await _context.DtCategories.AnyAsync(c => c.Category == categoryDto.Category && c.Id != id))
        {
            return BadRequest("Category name already exists");
        }

        category.Category = categoryDto.Category;
        category.Description = categoryDto.Description;
        category.TypeId = Int32.Parse(categoryDto.TypeId);
        category.LastUpdated = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
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

    // DELETE: api/Category/categories/5
    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.DtCategories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.DtCategories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoryExists(int id)
    {
        return _context.DtCategories.Any(e => e.Id == id);
    }
}

// DTOs for category operations
public class CategoryCreateDto
{
    public string Category { get; set; }
    public string Description { get; set; }
    public string TypeId { get; set; }
}

public class CategoryUpdateDto
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string TypeId { get; set; }
}
// PaginationParams.cs
public class PaginationParams
{
    private const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    // For search functionality
    public string? SearchTerm { get; set; } = string.Empty;
}
public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int? TypeId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastUpdated { get; set; }
    public int? RecordStatusId { get; set; }
}

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);

    public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
    }
}