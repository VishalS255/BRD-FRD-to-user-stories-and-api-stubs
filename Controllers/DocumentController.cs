using CapstoneProj.Models;
using CapstoneProj.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentParsingService _documentParsingService;
        private readonly TextChunkingService _textChunkingService;
        private readonly EmbeddingService _embeddingService;
        private readonly VectorStoreService _vectorStoreService;

        public DocumentController(
            DocumentParsingService documentParsingService,
            TextChunkingService textChunkingService,
            EmbeddingService embeddingService,
            VectorStoreService vectorStoreService)
        {
            _documentParsingService = documentParsingService;
            _textChunkingService = textChunkingService;
            _embeddingService = embeddingService;
            _vectorStoreService = vectorStoreService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedExtensions = new[] { ".txt", ".docx", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Unsupported file type.");

            if (file.Length > 20 * 1024 * 1024)
                return BadRequest("File too large. Max allowed size is 20 MB.");

            string content;
            try
            {
                content = await _documentParsingService.ExtractTextAsync(file);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("No readable text found in the uploaded file.");

            var document = new UploadedDocument
            {
                DocumentId = Guid.NewGuid().ToString(),
                FileName = file.FileName,
                Extension = extension,
                Size = file.Length,
                Content = content
            };

            _vectorStoreService.AddDocument(document);

            var chunks = _textChunkingService.ChunkText(document.DocumentId, file.FileName, content);

            foreach (var chunk in chunks)
            {
                chunk.Embedding = await _embeddingService.GenerateEmbeddingAsync(chunk.Text);
            }

            _vectorStoreService.AddChunks(chunks);

            return Ok(new
            {
                documentId = document.DocumentId,
                fileName = file.FileName,
                size = file.Length,
                characters = content.Length,
                chunkCount = chunks.Count,
                chunks = chunks.Select(c => new
                {
                    c.ChunkId,
                    c.ChunkIndex,
                    preview = c.Text.Substring(0, Math.Min(150, c.Text.Length))
                })
            });
        }

        [HttpGet("documents")]
        public IActionResult GetDocuments()
        {
            return Ok(_vectorStoreService.GetDocuments());
        }

        [HttpGet("chunks")]
        public IActionResult GetChunks()
        {
            return Ok(_vectorStoreService.GetChunks().Select(c => new
            {
                c.ChunkId,
                c.DocumentId,
                c.FileName,
                c.ChunkIndex,
                c.SectionTitle,
                Preview = c.Text.Substring(0, Math.Min(150, c.Text.Length))
            }));
        }
    }
}