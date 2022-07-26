# cert-schema-dotnet

The library implements a static document store for Blockcerts schemas for usage in .NET projects. 
The documents are loaded from @blockcerts/schemas npm package. Mapping is done based on parsing "preloadedContexts.js" file from this package.

## Usage

The main document store is implemented in `DocumentStore` class. 
It provides the `GetDocument` method used for resolving the requested URL to the stored document.

### Example

```
var store = new DocumentStore();
var document = store.GetDocument(url);
```

## Contribute

### Initialization

To initialize/update the @blockcerts/schemas npm package, run the following command:

```
$ npm install @blockcerts/schemas
```

### Run the tests

```
$ dotnet test .\code\Blockcerts.Schema.Test
```

## Contact

Contact us at [the Blockcerts community forum](http://community.blockcerts.org/).
