using System.ComponentModel.DataAnnotations;

namespace Orchestra.ATS.Domain;

public class Job
{
    public Guid Id { get; set; }

    [MaxLength(200)]
    public required string Title { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Application> Applications { get; set; } = new List<Application>();

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Job is already inactive.");
        }
        IsActive = false;
    }
}