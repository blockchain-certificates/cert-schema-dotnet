# cert-schema-dotnet

The library implements a static document store for Blockcerts schemas for usage in .NET projects. 
The documents are loaded from @blockcerts/schemas npm package. Mapping is done based on parsing "preloadedContexts.js" file from this package.

There is also a document loader implemented for usage in [jsonld-normalization-dotnet](https://github.com/blockchain-certificates/jsonld-normalization-dotnet) library.

## Usage

The main document store is implemented in `DocumentStore` class. 
It provides the `GetDocument` method used for resolving the requested URL to the stored document.

To use the `DocumentStore` class as a source for the stored document in [jsonld-normalization-dotnet](https://github.com/blockchain-certificates/jsonld-normalization-dotnet)'s `JsonLdHandler.Normalize` method, 
you have to pass an instance of `BlockcertsContextResolver` class to `ExpandOptions.ContextResolver` property.

### Example

```
var store = new DocumentStore();
var document = store.GetDocument(url);
```

```
var opts = new ExpandOptions
{
    ContextResolver = new BlockcertsContextResolver()
};
await JsonLdHandler.Normalize(File.ReadAllText(fileName), opts);
```

## Contribute

### Initialization

To initialize the @blockcerts/schemas npm package, run the following command:

```
$ npm install
```

### Run the tests

```
$ dotnet test .\code\Blockcerts.Schema.Test
```

## Contact

Contact us at [the Blockcerts community forum](http://community.blockcerts.org/).
