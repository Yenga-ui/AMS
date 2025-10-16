using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly AssetContext _context;

    public ProjectController(AssetContext context)
    {
        _context = context;
    }

    // GET: api/Project/projects
    [HttpGet("projects")]
    public async Task<ActionResult<IEnumerable<object>>> GetProjects()
    {
        var projects = await _context.DtProjects
            .Select(p => new
            {
                p.Id,
                p.Code,
                p.Project,
                p.CreatedDate,
                p.LastUpdated
            })
            .ToListAsync();

        return Ok(projects);
    }

    // GET: api/Project/projects/5
    [HttpGet("projects/{id}")]
    public async Task<ActionResult<object>> GetProject(int id)
    {
        var project = await _context.DtProjects
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.Code,
                p.Project,
                p.CreatedDate,
                p.LastUpdated
            })
            .FirstOrDefaultAsync();

        if (project == null)
        {
            return NotFound();
        }

        return project;
    }

    // POST: api/Project/projects
    [HttpPost("projects")]
    public async Task<ActionResult<object>> CreateProject(ProjectCreateDto projectDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if project code already exists
        if (await _context.DtProjects.AnyAsync(p => p.Code == projectDto.Code))
        {
            return BadRequest("Project code already exists");
        }

        // Check if project name already exists
        if (await _context.DtProjects.AnyAsync(p => p.Project == projectDto.Project))
        {
            return BadRequest("Project name already exists");
        }

        var project = new DtProject
        {
            Code = projectDto.Code,
            Project = projectDto.Project,
            CreatedDate = DateTime.Now,
            RecordStatusId = 7 // Active status
        };

        _context.DtProjects.Add(project);
        await _context.SaveChangesAsync();

        var createdProject = new
        {
            project.Id,
            project.Code,
            project.Project,
            project.CreatedDate
        };

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, createdProject);
    }

    // PUT: api/Project/projects/5
    [HttpPut("projects/{id}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectUpdateDto projectDto)
    {
        if (id != projectDto.Id)
        {
            return BadRequest();
        }

        var project = await _context.DtProjects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        // Check if project code already exists (excluding current project)
        if (await _context.DtProjects.AnyAsync(p => p.Code == projectDto.Code && p.Id != id))
        {
            return BadRequest("Project code already exists");
        }

        // Check if project name already exists (excluding current project)
        if (await _context.DtProjects.AnyAsync(p => p.Project == projectDto.Project && p.Id != id))
        {
            return BadRequest("Project name already exists");
        }

        project.Code = projectDto.Code;
        project.Project = projectDto.Project;
        project.LastUpdated = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectExists(id))
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

    // DELETE: api/Project/projects/5
    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.DtProjects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        _context.DtProjects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProjectExists(int id)
    {
        return _context.DtProjects.Any(e => e.Id == id);
    }
}

// DTOs for project operations
public class ProjectCreateDto
{
    public string Code { get; set; }
    public string Project { get; set; }
}

public class ProjectUpdateDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Project { get; set; }
}