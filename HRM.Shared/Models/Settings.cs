using System.ComponentModel.DataAnnotations;

namespace HRM.Shared.Models;

public class Currency : BaseEntity
{
    [Required, MaxLength(10)]
    public string Code { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(5)]
    public string Symbol { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AutoNumber : BaseEntity
{
    [Required, MaxLength(100)]
    public string EntityName { get; set; } = string.Empty;
    [Required, MaxLength(20)]
    public string Prefix { get; set; } = string.Empty;
    public int LastNumber { get; set; }
    public int NumberLength { get; set; } = 4;
    public string Generate() => $"{Prefix}-{(++LastNumber).ToString().PadLeft(NumberLength, '0')}";
}

public class SystemLog : BaseEntity
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? Source { get; set; }
    public string? UserId { get; set; }
}
