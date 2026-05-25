using CapstoneProj.Models;

namespace CapstoneProj.Services
{
    public class VectorStoreService
    {
        private readonly List<UploadedDocument> _documents = new();
        private readonly List<DocumentChunk> _chunks = new();
        private readonly List<RequirementFact> _requirementFacts = new();
        private readonly List<UserStory> _stories = new();
        private readonly List<ApiStub> _apiStubs = new();

        public void AddDocument(UploadedDocument document)
        {
            _documents.Add(document);
        }

        public List<UploadedDocument> GetDocuments()
        {
            return _documents;
        }

        public void AddChunks(IEnumerable<DocumentChunk> chunks)
        {
            _chunks.AddRange(chunks);
        }

        public List<DocumentChunk> GetChunks()
        {
            return _chunks;
        }

        public void AddRequirementFacts(IEnumerable<RequirementFact> facts)
        {
            _requirementFacts.AddRange(facts);
        }

        public List<RequirementFact> GetRequirementFacts()
        {
            return _requirementFacts;
        }

        public void AddStories(IEnumerable<UserStory> stories)
        {
            _stories.AddRange(stories);
        }

        public List<UserStory> GetStories()
        {
            return _stories;
        }

        public void AddApiStubs(IEnumerable<ApiStub> stubs)
        {
            _apiStubs.AddRange(stubs);
        }

        public List<ApiStub> GetApiStubs()
        {
            return _apiStubs;
        }
    }
}