namespace Backend.Messages;

// RabbitMQ message sent when a new prompt is created
public record PromptCreated(Guid PromptId);