using System.ComponentModel.DataAnnotations;

// These two records should already be here
public record CreateJobRequest([Required] string Title, [Required] string Description);
public record JobResponse(Guid Id, string Title, string Description, bool IsActive, DateTime CreatedAt);

// --- ADD THIS NEW RECORD ---
/// <summary>
/// Represents the data submitted when a candidate applies for a job.
/// </summary>
public record ApplyToJobRequest(
    [Required][EmailAddress] string CandidateEmail,
    [Required][MaxLength(100)] string FirstName,
    [Required][MaxLength(100)] string LastName
);