using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OweMeApi.Data.Entities;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [JsonIgnore]
    public string Hash { get; set; } = string.Empty;
}