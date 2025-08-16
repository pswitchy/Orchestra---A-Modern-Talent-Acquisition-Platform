using System.ComponentModel.DataAnnotations;

namespace Orchestra.IAM.Api.Auth;

// Record for the API request
public record LoginRequest([Required][EmailAddress] string Email, [Required] string Password);

// Record for the API response
public record LoginResponse(string Token);