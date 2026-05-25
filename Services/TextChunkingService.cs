using System.Text;
using CapstoneProj.Models;

namespace CapstoneProj.Services
{
    public class TextChunkingService
    {
        public List<DocumentChunk> ChunkText(string documentId, string fileName, string content, int maxChunkLength = 1000, int overlapLength = 100)
        {
            var chunks = new List<DocumentChunk>();

            if (string.IsNullOrWhiteSpace(content))
                return chunks;

            var paragraphs = content
                .Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var currentChunk = new StringBuilder();
            var chunkIndex = 0;

            foreach (var paragraph in paragraphs)
            {
                if (currentChunk.Length + paragraph.Length + 2 > maxChunkLength && currentChunk.Length > 0)
                {
                    var chunkText = currentChunk.ToString().Trim();

                    chunks.Add(new DocumentChunk
                    {
                        ChunkId = Guid.NewGuid().ToString(),
                        DocumentId = documentId,
                        FileName = fileName,
                        ChunkIndex = chunkIndex++,
                        SectionTitle = "Unknown",
                        Text = chunkText
                    });

                    var overlap = chunkText.Length > overlapLength
                        ? chunkText.Substring(chunkText.Length - overlapLength)
                        : chunkText;

                    currentChunk.Clear();
                    currentChunk.AppendLine(overlap);
                }

                currentChunk.AppendLine(paragraph);
                currentChunk.AppendLine();
            }

            if (currentChunk.Length > 0)
            {
                chunks.Add(new DocumentChunk
                {
                    ChunkId = Guid.NewGuid().ToString(),
                    DocumentId = documentId,
                    FileName = fileName,
                    ChunkIndex = chunkIndex,
                    SectionTitle = "Unknown",
                    Text = currentChunk.ToString().Trim()
                });
            }

            return chunks;
        }
    }
}