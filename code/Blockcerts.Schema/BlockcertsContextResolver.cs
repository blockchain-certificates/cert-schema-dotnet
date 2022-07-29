using JsonLd.Normalization;

namespace Blockcerts.Schema
{
    public class BlockcertsContextResolver : ContextResolver
    {
        protected DocumentStore documentStore;

        public BlockcertsContextResolver()
        {
            documentStore = new();
        }

        protected override Task<(string, string)> LoadDocument(string url, List<string>? redirects = null)
        {
            var stored = documentStore.GetDocument(url);
            if (!String.IsNullOrEmpty(stored))
                return Task.FromResult((stored, url));

            return base.LoadDocument(url, redirects);
        }
    }
}
