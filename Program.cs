using CapstoneProj.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new Exception("OPENAI_API_KEY not found in environment variables.");
}

builder.Services.AddSingleton(new OpenAIClient(apiKey));

builder.Services.AddSingleton<DocumentParsingService>();
builder.Services.AddSingleton<TextChunkingService>();
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<VectorStoreService>();
builder.Services.AddSingleton<RequirementExtractionService>();
builder.Services.AddSingleton<RetrievalService>();
builder.Services.AddSingleton<StoryGenerationService>();
builder.Services.AddSingleton<ApiStubGenerationService>();
builder.Services.AddSingleton<ExportService>();
builder.Services.AddSingleton<MetricsService>();
builder.Services.AddSingleton<ReviewerChecklistService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();