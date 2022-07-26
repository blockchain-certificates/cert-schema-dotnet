using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Blockcerts.Schema
{
    public class DocumentStore
    {
        protected const string URLS_FILE = "schemas\\context_urls.json";
        protected const string CONTEXTS_FILE = "preloadedContexts.js";
        protected const string RESOURCE_PREFIX = "./";

        protected readonly Dictionary<string, string> _cache = new();
        protected readonly Regex _importContextRegex = new Regex(@"import (\w*) from '([\w\\/\.]*)'");
        protected readonly Regex _preloadedContextsRegex = new Regex(@"preloadedContexts\[CONTEXT_URLS\.(\w*)\] = (\w*)");

        public DocumentStore()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var labelToUrlDict = MapLabelToUrl(thisAssembly);
            var contextsFile = LoadContextsFile(thisAssembly);
            var labelToDocumentDict = MapLabelToDocument(contextsFile);
            var urlToDocumentDict = MapUrlToDocument(contextsFile);
            InitCache(labelToUrlDict, labelToDocumentDict, urlToDocumentDict);
        }

        protected Dictionary<string, string> MapLabelToUrl(Assembly thisAssembly)
        {
            using var urlStream = thisAssembly.GetManifestResourceStream(URLS_FILE);
            if (urlStream is null)
                throw new FileNotFoundException($"Cannot find {URLS_FILE} file!");

            var urls = new Dictionary<string, string>();
            var urlsList = JsonDocument.Parse(urlStream).RootElement;
            foreach (var prop in urlsList.EnumerateObject())
                urls.Add(prop.Name, prop.Value.GetString()!);
            return urls;
        }

        protected string LoadContextsFile(Assembly thisAssembly)
        {
            using var contextsStream = thisAssembly.GetManifestResourceStream(CONTEXTS_FILE);
            if (contextsStream is null)
                throw new FileNotFoundException($"Cannot find {CONTEXTS_FILE} file!");

            using var reader = new StreamReader(contextsStream);
            return reader.ReadToEnd();
        }

        protected Dictionary<string, string> MapLabelToDocument(string preloadedContexts)
        {
            var labelToDocumentDict = new Dictionary<string, string>();
            foreach (Match match in _importContextRegex.Matches(preloadedContexts))
                labelToDocumentDict.Add(match.Groups[1].Value, match.Groups[2].Value);
            return labelToDocumentDict;
        }

        protected Dictionary<string, string> MapUrlToDocument(string preloadedContexts)
        {
            var urlToDocumentDict = new Dictionary<string, string>();
            foreach (Match match in _preloadedContextsRegex.Matches(preloadedContexts))
                urlToDocumentDict.Add(match.Groups[1].Value, match.Groups[2].Value);
            return urlToDocumentDict;
        }

        protected void InitCache(Dictionary<string, string> labelToUrlDict, Dictionary<string, string> labelToDocumentDict, 
                                 Dictionary<string, string> urlToDocumentDict)
        {
            foreach (var urlToDocument in urlToDocumentDict)
            {
                if (!labelToUrlDict.TryGetValue(urlToDocument.Key, out var url))
                    throw new FileNotFoundException($"Could not find url for '{urlToDocument.Key}' label");

                if (!labelToDocumentDict.TryGetValue(urlToDocument.Value, out var document))
                    throw new FileNotFoundException($"Could not find document for '{urlToDocument.Value}' label");

                var prefixIndex = document.IndexOf(RESOURCE_PREFIX);
                if (prefixIndex == 0)
                    document = document.Substring(RESOURCE_PREFIX.Length);
                var resource = document.Replace('/', '\\');

                _cache.Add(url, resource);
            }
        }

        /// <summary>
        /// Returns the stored document associated with the URL given, or null if not found
        /// </summary>
        /// <param name="url">URL for which to return the stored document</param>
        /// <returns>document text or null</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public string? GetDocument(string url)
        {
            if (!_cache.TryGetValue(url, out var resource))
                return null;

            var thisAssembly = Assembly.GetExecutingAssembly();            
            using var stream = thisAssembly.GetManifestResourceStream(resource);
            if (stream is null)
                throw new FileNotFoundException($"Could not find {resource} resource");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
