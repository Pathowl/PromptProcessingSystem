using Backend.Database;
using Backend.Entities;
using Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Backend.Messages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromptsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    // Wstrzykujemy bazę (do zapisu) jak i endpoint kolejki (do wysylki)
    public PromptsController(AppDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // fetching from database
        var prompts = await _context.Prompts.ToListAsync();
        return Ok(prompts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromptRequest request)
    {
        var prompt = new Prompt 
        { 
            Content = request.Content,
            Status = PromptStatus.Pending 
        };

        _context.Prompts.Add(prompt);
        await _context.SaveChangesAsync();

        // new task for rabbitmq
        await _publishEndpoint.Publish(new PromptCreated(prompt.Id));

        return Ok(prompt);
    }
    
}
