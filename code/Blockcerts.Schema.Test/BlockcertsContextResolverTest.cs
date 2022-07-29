using JsonLd.Normalization;
using System.Text;

namespace Blockcerts.Schema.Test
{
    internal class BlockcertsContextResolverTest
    {
        private readonly string[] _contexts = new[]
        {
            "https://www.blockcerts.org/schema/3.1/context.json",
            "https://w3id.org/blockcerts/schema/3.0/context.json",
            "https://w3id.org/openbadges/v2",
            "https://w3id.org/security/suites/chained-2021/v1",
            "https://www.w3.org/2018/credentials/v1"
        };

        [Test]
        public void ResolveCachedContexts_Success()
        {
            var sb = new StringBuilder("{\"@context\":[");
            bool addComma = false;
            foreach (var context in _contexts)
            {
                if (addComma)
                    sb.Append(",");
                else
                    addComma = true;
                sb.Append($"\"{context}\"");
            }
            sb.Append("]}");

            Assert.DoesNotThrowAsync(async () => //it's hard to test if the actual overriden method is invoked
            {                                    //instead of ContextResolver default HTTP request, this has to do for now
                var opts = new ExpandOptions
                {
                    ContextResolver = new BlockcertsContextResolver()
                };
                await JsonLdHandler.Normalize(sb.ToString(), opts);
            });
        }
    }
}
