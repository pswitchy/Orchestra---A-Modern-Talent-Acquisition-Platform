using System.ComponentModel.DataAnnotations;

namespace Orchestra.ATS.Api.Candidates.Models;

/// <summary>
/// The data required to create a new candidate in the system.
/// </summary>
public record CreateCandidateRequest(
    [Required][EmailAddress] string Email,
    [Required][MaxLength(100)] string FirstName,
    [Required][MaxLength(100)] string LastName
);

/// <summary>
/// The data that is returned to the client when querying for a candidate.
/// </summary>
public record CandidateResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName
);