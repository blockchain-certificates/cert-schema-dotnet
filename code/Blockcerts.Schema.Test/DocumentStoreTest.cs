using System.Reflection;

namespace Blockcerts.Schema.Test
{
    public class DocumentStoreTest
    {
        [TestCase("https://www.blockcerts.org/schema/3.1/context.json", "schemas\\3.1\\context.json")]
        [TestCase("https://w3id.org/blockcerts/schema/3.0/context.json", "schemas\\3.0\\context.json")]
        [TestCase("https://w3id.org/openbadges/v2", "schemas\\2.1\\obi.json")]
        [TestCase("https://w3id.org/security/suites/chained-2021/v1", "schemas\\3.1\\chainedProof2021Context.json")]
        [TestCase("https://www.w3.org/2018/credentials/v1", "schemas\\3.0\\credential.json")]
        public void GetDocument_Success(string url, string documentName)
        {
            string expected = "";
            {
                using var stream = Assembly.GetAssembly(typeof(DocumentStore))!.GetManifestResourceStream(documentName);
                using var reader = new StreamReader(stream!);
                expected = reader.ReadToEnd();
            }
            var store = new DocumentStore();

            var document = store.GetDocument(url);

            Assert.IsNotNull(document);
            Assert.IsNotEmpty(document);
            Assert.AreEqual(expected, document);
        }
    }
}