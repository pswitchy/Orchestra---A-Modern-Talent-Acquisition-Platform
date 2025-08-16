using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.ATS.Domain;
using Orchestra.ATS.Infrastructure;

namespace Orchestra.ATS.Api.Jobs;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly AtsDbContext _context;

    public JobsController(AtsDbContext context)
    {
        _context = context;
    }

    // --- EXISTING METHODS (GetAll, Create) GO HERE ---

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var jobs = await _context.Jobs
            .Where(j => j.IsActive)
            .Select(j => new JobResponse(j.Id, j.Title, j.Description, j.IsActive, j.CreatedAt))
            .ToListAsync();
        return Ok(jobs);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateJobRequest request)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description
        };
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        var response = new JobResponse(job.Id, job.Title, job.Description, job.IsActive, job.CreatedAt);
        return CreatedAtAction(nameof(GetAll), new { id = job.Id }, response);
    }


    // --- ADD THIS NEW ENDPOINT ---

    /// <summary>
    /// Allows a candidate to apply for a specific job.
    /// If the candidate does not exist, they will be created.
    /// </summary>
    [HttpPost("{jobId:guid}/apply")]
    [AllowAnonymous] // Anyone can apply for a job, they don't need to be logged in.
    public async Task<IActionResult> Apply(Guid jobId, ApplyToJobRequest request)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId && j.IsActive);
        if (job is null)
        {
            return NotFound("Job not found or is no longer active.");
        }

        // Find-or-Create pattern for the candidate
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.Email == request.CandidateEmail);

        if (candidate is null)
        {
            candidate = new Candidate
            {
                Id = Guid.NewGuid(),
                Email = request.CandidateEmail,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            _context.Candidates.Add(candidate);
        }

        // Check for duplicate applications
        var alreadyApplied = await _context.Applications
            .AnyAsync(a => a.JobId == jobId && a.CandidateId == candidate.Id);

        if (alreadyApplied)
        {
            return Conflict("This candidate has already applied for this job.");
        }

        var application = new Application
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            CandidateId = candidate.Id,
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync(); // Saves the new candidate (if any) and the application together

        return Ok("Application successful.");
    }
}