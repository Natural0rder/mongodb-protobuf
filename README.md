A simple .NET 7 console app to test MongoDB fetch performance in various conditions:

- Classic BSON
- Protobuf (encapsulated in an indexable enveloppe)
- GZipped Protobuf

Tests are run with various network compression: Zlib, Zstd, Snappy and no network compression.

<img width="1179" alt="image" src="https://github.com/Natural0rder/mongodb-protobuf/assets/102281652/c823f859-7ae8-4f52-a91b-a20f0ac4a6c8">

Use appsettings.json to configure:

- The preparation of the data set (PrepareDataset)
- The fetch count per run (FetchCountPerRun)

Adapt POCO.cs to build your own test classes.
