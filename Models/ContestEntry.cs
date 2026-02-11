using System.ComponentModel.DataAnnotations;

public class ContestEntry
{
    public int Id { get; set; }

    [Required]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    public string AccountName { get; set; } = string.Empty;

    [Required]
    public decimal LowerRate { get; set; }

    [Required]
    public decimal UpperRate { get; set; }

    public bool IsWinner { get; set; }

    public DateTime? WonAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
