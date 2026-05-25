using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;

namespace CapstoneProj.Services
{
    public class DocumentParsingService
    {
        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension == ".txt")
            {
                using var reader = new StreamReader(file.OpenReadStream());
                return await reader.ReadToEndAsync();
            }

            if (extension == ".docx")
            {
                return await ReadDocxTextAsync(file);
            }

            if (extension == ".pdf")
            {
                throw new NotSupportedException("PDF parsing not added yet.");
            }

            throw new NotSupportedException("Unsupported file type.");
        }

        private static async Task<string> ReadDocxTextAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var wordDocument = WordprocessingDocument.Open(memoryStream, false);
            var body = wordDocument.MainDocumentPart?.Document?.Body;

            if (body == null)
            {
                return string.Empty;
            }

            var paragraphs = body.Elements<Paragraph>()
                .Select(p => p.InnerText)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            return string.Join(Environment.NewLine + Environment.NewLine, paragraphs);
        }
    }
}