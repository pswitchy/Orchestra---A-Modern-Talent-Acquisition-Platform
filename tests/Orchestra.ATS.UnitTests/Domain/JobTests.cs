using Orchestra.ATS.Domain;

namespace Orchestra.ATS.UnitTests.Domain;

public class JobTests
{
    [Fact]
    public void Deactivate_WhenJobIsActive_SetsIsActiveToFalse()
    {
        // Arrange
        var job = new Job { Title = "Test Job", Description = "A job for testing." };

        // Act
        job.Deactivate();

        // Assert
        Assert.False(job.IsActive);
    }

    [Fact]
    public void Deactivate_WhenJobIsAlreadyInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var job = new Job { Title = "Test Job", Description = "A job for testing.", IsActive = false };

        // Act
        var action = () => job.Deactivate();

        // Assert
        Assert.Throws<InvalidOperationException>(action);
    }
}