using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.ATS.Api.Candidates.Models;
using Orchestra.ATS.Domain;
using Orchestra.ATS.Infrastructure;

namespace Orchestra.ATS.Api.Candidates;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Securing these endpoints is important.
public class CandidatesController : ControllerBase
{
    private readonly AtsDbContext _context;

    public CandidatesController(AtsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new candidate.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")] // Only Admins can manually add candidates
    public async Task<IActionResult> Create(CreateCandidateRequest request)
    {
        if (await _context.Candidates.AnyAsync(c => c.Email == request.Email))
        {
            return Conflict("A candidate with this email already exists.");
        }

        var candidate = new Candidate
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();

        var response = new CandidateResponse(candidate.Id, candidate.Email, candidate.FirstName, candidate.LastName);

        return CreatedAtAction(nameof(GetById), new { id = candidate.Id }, response);
    }

    /// <summary>
    /// Retrieves a list of all candidates.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var candidates = await _context.Candidates
            .Select(c => new CandidateResponse(c.Id, c.Email, c.FirstName, c.LastName))
            .ToListAsync();

        return Ok(candidates);
    }

    /// <summary>
    /// Retrieves a specific candidate by their ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var candidate = await _context.Candidates.FindAsync(id);

        if (candidate is null)
        {
            return NotFound();
        }

        var response = new CandidateResponse(candidate.Id, candidate.Email, candidate.FirstName, candidate.LastName);
        return Ok(response);
    }
}