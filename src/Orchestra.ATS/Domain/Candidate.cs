using System.ComponentModel.DataAnnotations;

namespace Orchestra.ATS.Domain;

public class Candidate
{
    public Guid Id { get; set; }
    [MaxLength(256)]
    public required string Email { get; set; }
    [MaxLength(100)]
    public required string FirstName { get; set; }
    [MaxLength(100)]
    public required string LastName { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}