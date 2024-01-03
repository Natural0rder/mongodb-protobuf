An uncomplicated .NET 7 console application designed to evaluate MongoDB retrieval performance across diverse scenarios:

1. **Classic BSON:** Examining fetch performance using the traditional BSON format.
   
2. **Protobuf (encapsulated in an indexable envelope):** Assessing the efficiency of data retrieval when utilizing Protobuf serialization, encapsulated within an indexable envelope for enhanced organization.

3. **GZipped Protobuf:** Investigating the impact of employing Protobuf serialization with an additional layer of compression through GZip. This aims to analyze the combined performance benefits of Protobuf's compactness and GZip's compression.

The tests encompass a spectrum of network compression methodologies, including:

- **Zlib:** Evaluating MongoDB fetch performance with data transmitted using Zlib compression.
  
- **Zstd:** Investigating the effects of utilizing Zstandard (Zstd) compression on data retrieval from MongoDB.

- **Snappy:** Assessing the performance of MongoDB fetch operations when the data is compressed using the Snappy compression algorithm.

- **No Network Compression:** Establishing a baseline by conducting tests without any network compression, allowing for a comparison against compressed scenarios.

This comprehensive evaluation provides insights into the comparative performance of MongoDB fetch operations under different serialization formats and compression strategies, aiding in the optimization of data retrieval processes based on specific use cases and requirements.

<img width="1179" alt="image" src="https://github.com/Natural0rder/mongodb-protobuf/assets/102281652/c823f859-7ae8-4f52-a91b-a20f0ac4a6c8">

Optimize your application with appsettings.json for streamlined configuration:

1. **Data Set Preparation (PrepareDataset):** Tailor the data set preparation effortlessly by adjusting parameters in appsettings.json, ensuring adaptability to specific testing needs.

2. **Fetch Count per Run (FetchCountPerRun):** Dynamically set the number of fetch operations through appsettings.json, providing flexibility for varied performance testing scenarios.

3. **Custom Test Classes (Adapt POCO.cs):** Modify POCO.cs to construct personalized test classes, aligning your application precisely with unique testing requirements.
