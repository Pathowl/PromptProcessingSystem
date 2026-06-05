namespace Backend.Entities;

public class Prompt
{
    // ID
    public Guid Id { get; set; } = Guid.NewGuid();

    // Content
    public required string Content { get; set; }

    // Status
    public PromptStatus Status { get; set; } = PromptStatus.Pending;

    // Result
    public string? Result { get; set; }

    // Date
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    
    // Error
    public string? ErrorMessage { get; set; }
}