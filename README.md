# Summary

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

<img width="1178" alt="image" src="https://github.com/Natural0rder/mongodb-protobuf/assets/102281652/49e4ef45-bd6c-44f3-bff2-6022767f5b89">

# Configuration

Optimize your application with appsettings.json for streamlined configuration:

1. **Data Set Preparation (PrepareDataset):** Tailor the data set preparation effortlessly by adjusting parameters in appsettings.json, ensuring adaptability to specific testing needs.

2. **Fetch Count per Run (FetchCountPerRun):** Dynamically set the number of fetch operations through appsettings.json, providing flexibility for varied performance testing scenarios.

3. **Custom Test Classes (Adapt POCO.cs):** Modify POCO.cs to construct personalized test classes, aligning your application precisely with unique testing requirements.

# Test conditions

Avoid depending solely on tests conducted on your local machine. Instead, prioritize running the application in a virtual machine (VM) or a container with a network configuration that closely mirrors real-world conditions and is in proximity to your MongoDB cluster.

To install dotnet on AWS EC2 (Amazon Linux):

**https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#scripted-install
**https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#set-environment-variables-system-wide

In case of globalization concern:

```
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
```

Then, once in the .csproj folder:

```
dotnet build ./
dotnet run ./
```

# Console output

Here is an output sample with 2 runs per configuration (performing a warm up is relevant to avoid "first slow query" behavior):

```
Run with [Snappy] network compression.
Start 2 run(s) with [BSON].
Fetch #1: 251 ms to [fetch] 100 documents.
Fetch #2: 68 ms to [fetch] 100 documents.
Start 2 run(s) with [Protobuf].
Fetch #1: 119 ms to [fetch] 100 documents.
Post-processing #1: 47 ms to [deserialize] 100 documents.
Fetch #2: 73 ms to [fetch] 100 documents.
Post-processing #2: 0 ms to [deserialize] 100 documents.
Start 2 run(s) with [GZipped Protobuf].
Fetch #1: 126 ms to [fetch] 100 documents.
Post-processing #1: 3 ms to [deserialize and unzip] 100 documents.
Fetch #2: 85 ms to [fetch] 100 documents.
Post-processing #2: 1 ms to [deserialize and unzip] 100 documents.
Done.
Run with [Zlib] network compression.
Start 2 run(s) with [BSON].
Fetch #1: 159 ms to [fetch] 100 documents.
Fetch #2: 104 ms to [fetch] 100 documents.
Start 2 run(s) with [Protobuf].
Fetch #1: 86 ms to [fetch] 100 documents.
Post-processing #1: 0 ms to [deserialize] 100 documents.
Fetch #2: 115 ms to [fetch] 100 documents.
Post-processing #2: 0 ms to [deserialize] 100 documents.
Start 2 run(s) with [GZipped Protobuf].
Fetch #1: 110 ms to [fetch] 100 documents.
Post-processing #1: 0 ms to [deserialize and unzip] 100 documents.
Fetch #2: 117 ms to [fetch] 100 documents.
Post-processing #2: 0 ms to [deserialize and unzip] 100 documents.
Done.
Run with [Zstd] network compression.
Start 2 run(s) with [BSON].
Fetch #1: 170 ms to [fetch] 100 documents.
Fetch #2: 109 ms to [fetch] 100 documents.
Start 2 run(s) with [Protobuf].
Fetch #1: 74 ms to [fetch] 100 documents.
Post-processing #1: 0 ms to [deserialize] 100 documents.
Fetch #2: 114 ms to [fetch] 100 documents.
Post-processing #2: 2 ms to [deserialize] 100 documents.
Start 2 run(s) with [GZipped Protobuf].
Fetch #1: 100 ms to [fetch] 100 documents.
Post-processing #1: 2 ms to [deserialize and unzip] 100 documents.
Fetch #2: 78 ms to [fetch] 100 documents.
Post-processing #2: 2 ms to [deserialize and unzip] 100 documents.
Done.
Run with [No] network compression.
Start 2 run(s) with [BSON].
Fetch #1: 176 ms to [fetch] 100 documents.
Fetch #2: 157 ms to [fetch] 100 documents.
Start 2 run(s) with [Protobuf].
Fetch #1: 152 ms to [fetch] 100 documents.
Post-processing #1: 4 ms to [deserialize] 100 documents.
Fetch #2: 131 ms to [fetch] 100 documents.
Post-processing #2: 3 ms to [deserialize] 100 documents.
Start 2 run(s) with [GZipped Protobuf].
Fetch #1: 80 ms to [fetch] 100 documents.
Post-processing #1: 0 ms to [deserialize and unzip] 100 documents.
Fetch #2: 87 ms to [fetch] 100 documents.
Post-processing #2: 0 ms to [deserialize and unzip] 100 documents.
Done.
```
