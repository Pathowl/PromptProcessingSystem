using Backend.Database;
using Backend.Entities;
using Backend.Messages;
using Backend.Services;
using MassTransit;

namespace Backend.Workers;

// only promptcreated
public class PromptWorker : IConsumer<PromptCreated>
{
    private readonly AppDbContext _context;
    private readonly GeminiService _aiService;

    // Worker db access
    public PromptWorker(AppDbContext context, GeminiService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

 public async Task Consume(ConsumeContext<PromptCreated> context)
{
    var promptId = context.Message.PromptId;
    Console.WriteLine($"\n[WORKER] Wyłapałem nowe zadanie z RabbitMQ! ID: {promptId}");

    var prompt = await _context.Prompts.FindAsync(promptId);
    if (prompt == null) 
    {
        Console.WriteLine("[WORKER] Błąd: Nie znaleziono prompta w bazie!");
        return;
    }

    try 
    {
        // Zmiana na Processing
        prompt.Status = PromptStatus.Processing;
        await _context.SaveChangesAsync();
        Console.WriteLine($"[WORKER] Status zmieniony na Processing. Uruchamiam symulację AI...");

        // Symulacja pracy AI
        await Task.Delay(5000); 

        // Zapisanie wyniku 
        prompt.Result = await _aiService.ProcessPromptAsync(prompt.Content);;
        prompt.Status = PromptStatus.Completed;
    }
    catch (Exception ex)
    {
        // Obsluga bledu
        Console.WriteLine($"[WORKER] BŁĄD przy zadaniu {promptId}: {ex.Message}");
        
        prompt.Status = PromptStatus.Failed;
        prompt.ErrorMessage = ex.Message; // saving error
    }
    finally
    {
        // database save
        await _context.SaveChangesAsync();
        Console.WriteLine($"[WORKER] Zakończono obsługę zadania {promptId}\n");
    }
}}