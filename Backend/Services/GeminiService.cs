using System.Text.Json;

namespace Backend.Services;
using System.Text.Json;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GeminiApiKey"] ?? "";
    }

    public async Task<string> ProcessPromptAsync(string userMessage)
    {
        // simulation
        await Task.Delay(5000); // AI thinking time
        
        return $"[AI Mock Response]\nOtrzymałem Twoją wiadomość: \"{userMessage}\".\n\n" +
               $"System działa w trybie symulacji. Infrastruktura przetwarzania w tle " +
               $"została zweryfikowana pomyślnie. Dodaj klucz API Gemini, aby zobaczyć prawdziwe odpowiedzi generowane przez model AI.";
    }
}
// public class GeminiService
// {
//     private readonly HttpClient _httpClient;
//     private readonly string _apiKey;

//     public GeminiService(HttpClient httpClient, IConfiguration configuration)
//     {
//         _httpClient = httpClient;
//         _apiKey = configuration["GeminiApiKey"] ?? "";
//     }

//     public async Task<string> ProcessPromptAsync(string userMessage)
//     {
//         var cleanApiKey = _apiKey?.Trim(); 

//         if (string.IsNullOrEmpty(cleanApiKey) || cleanApiKey == "TUTAJ_WKLEJ_SWOJ_KLUCZ_Z_GOOGLE") 
//         {
//             await Task.Delay(5000);
//             return $"[Symulacja AI] Otrzymałem Twoją wiadomość: '{userMessage}'. Dodaj klucz API, by zobaczyć prawdziwą odpowiedź.";
//         }

//         var systemPrompt = $"""
//             You are a highly efficient, helpful AI assistant running within an asynchronous background processing system.
//             Your task is to analyze and respond to the user's message.

//             User Message:
//             "{userMessage}"

//             Task & Rules:
//             1. Provide a direct, clear, and helpful response.
//             2. Do not write filler text like "Here is your answer".
//             3. Answer in the exact same language the user used.
//             4. Format your response clearly using line breaks if necessary.
//             5. Don't use more than 100 words. Be concise and to the point.
//             """;
            
//         var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={cleanApiKey}";
        
//         var payload = new {
//             contents = new[] { new { parts = new[] { new { text = systemPrompt } } } }
//         };

//         var response = await _httpClient.PostAsJsonAsync(url, payload);
        
//         // error
//         if (!response.IsSuccessStatusCode)
//         {
//             var errorBody = await response.Content.ReadAsStringAsync();
//             throw new Exception($"BŁĄD GOOGLE: {errorBody}");
//         }

//         var data = await response.Content.ReadFromJsonAsync<JsonElement>();
        
//         return data.GetProperty("candidates")[0]
//                    .GetProperty("content")
//                    .GetProperty("parts")[0]
//                    .GetProperty("text").GetString() ?? "No response";
//     }
// }

