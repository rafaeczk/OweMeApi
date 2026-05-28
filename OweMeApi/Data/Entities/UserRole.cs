using System.ComponentModel.DataAnnotations;

namespace OweMeApi.Data.Entities;

public class UserRole
{
    [Key]
    public string Code { get; set; } = string.Empty;
    [Required]
    public string Label { get; set; } = string.Empty;
}

