using System.Diagnostics;
using LoadProtobufSample;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

const string DB_NAME = "protobuf";
const string COLL_NAME = "dummy-bson";
const string COLL_NAME_PB = "dummy-protobuf";
const string COLL_NAME_ZIP_PB = "dummy-protobuf-zipped";
const int DOC_COUNT = 10000;
var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
var config = builder.Build();
var connectionString = config["ConnectionStrings:DefaultConnection"];

if (null == connectionString)
    throw new NullReferenceException("No connection string provided in appsettings.json.");

bool prepareDataset = false;
bool.TryParse(config["PrepareDataset"], out prepareDataset);

int fetchCountPerRun = 1;
int.TryParse(config["FetchCountPerRun"], out fetchCountPerRun);

if (fetchCountPerRun < 1)
    fetchCountPerRun = 1;

if (prepareDataset)
    PrepareDataset(connectionString, DOC_COUNT);

Run(connectionString, fetchCountPerRun, NetworkCompression.Snappy);
Run(connectionString, fetchCountPerRun, NetworkCompression.Zlib);
Run(connectionString, fetchCountPerRun, NetworkCompression.Zstd);
Run(connectionString, fetchCountPerRun);

Console.ReadLine();

void RunFetch<T>(IMongoCollection<T> coll, int iteration, bool unzip = false)
{
    var sw = new Stopwatch();
    sw.Start();

    // Build your MongoDB query here
    var docs = coll.Find(Builders<T>.Filter.Empty).ToList();

    var elapsedFetch = sw.ElapsedMilliseconds;
    Console.WriteLine($"Fetch #{iteration + 1}: {elapsedFetch} ms to [fetch] {docs.Count} documents.");

    if (typeof(T) == typeof(DummyForProtobuf))
    {
        var task = unzip ? "[deserialize and unzip]" : "[deserialize]";

        sw.Reset();
        sw.Start();

        Parallel.ForEach(docs, doc =>
        {
            if (doc != null)
            {
                var d = (DummyForProtobuf)Convert.ChangeType(doc, typeof(DummyForProtobuf));
                if (d != null)
                {
                    d.Content.FromProtobuf(unzip);
                }
            }
        });

        var elapsedDeserialize = sw.ElapsedMilliseconds;
        Console.WriteLine($"Post-processing #{iteration + 1}: {elapsedDeserialize} ms to {task} {docs.Count} documents.");
    }
}

void Run(string cs, int fetchCount, NetworkCompression? networkCompression = null)
{
    switch (networkCompression)
    {
        case NetworkCompression.Zlib:
            cs = cs + "/?compressors=zlib";
            Console.WriteLine("Run with [Zlib] network compression.");
            break;
        case NetworkCompression.Zstd:
            cs = cs + "/?compressors=zstd";
            Console.WriteLine("Run with [Zstd] network compression.");
            break;
        case NetworkCompression.Snappy:
            cs = cs + "/?compressors=snappy";
            Console.WriteLine("Run with [Snappy] network compression.");
            break;
        default:
            Console.WriteLine("Run with [No] network compression.");
            break;
    }

    var client = new MongoClient(cs);
    var database = client.GetDatabase(DB_NAME);

    // You can add a warm up process here (eg. to avoid first "slow" query for instance)
    // RunWarmUp();

    Console.WriteLine($"Start {fetchCount} run(s) with [BSON].");
    for (int i = 0; i < fetchCount; i++)
        RunFetch(database.GetCollection<DummyForBson>(COLL_NAME), i);
    Console.WriteLine($"Start {fetchCount} run(s) with [Protobuf].");
    for (int i = 0; i < fetchCount; i++)
        RunFetch(database.GetCollection<DummyForProtobuf>(COLL_NAME_PB), i);
    Console.WriteLine($"Start {fetchCount} run(s) with [GZipped Protobuf].");
    for (int i = 0; i < fetchCount; i++)
        RunFetch(database.GetCollection<DummyForProtobuf>(COLL_NAME_ZIP_PB), i, true);
    Console.WriteLine("Done.");
}

void PrepareDataset(string cs, int docCount)
{
    var client = new MongoClient(cs);
    var database = client.GetDatabase(DB_NAME);
    var collectionBson = database.GetCollection<DummyForBson>(COLL_NAME);
    var collectionProtobuf = database.GetCollection<DummyForProtobuf>(COLL_NAME_PB);
    var collectionProtobufZipped = database.GetCollection<DummyForProtobuf>(COLL_NAME_ZIP_PB);
    Console.WriteLine("Preparing test data set.");

    var dummyBson = new List<DummyForBson>();

    for (int i = 0; i < docCount; i++)
        dummyBson.Add(new DummyForBson());

    var dummyProtobuf = new List<DummyForProtobuf>();

    for (int i = 0; i < docCount; i++)
        dummyProtobuf.Add(new DummyForProtobuf());

    var dummyProtobufZipped = new List<DummyForProtobuf>();

    for (int i = 0; i < docCount; i++)
        dummyProtobufZipped.Add(new DummyForProtobuf(true));

    Console.WriteLine($"Persisting {docCount} documents with [BSON].");
    database.DropCollection(COLL_NAME);
    collectionBson.InsertMany(dummyBson);

    Console.WriteLine($"Persisting {docCount} documents with [Protobuf].");
    database.DropCollection(COLL_NAME_PB);
    collectionProtobuf.InsertMany(dummyProtobuf);

    Console.WriteLine($"Persisting {docCount} documents with [GZipped Protobuf].");
    database.DropCollection(COLL_NAME_ZIP_PB);
    collectionProtobufZipped.InsertMany(dummyProtobufZipped);

    Console.WriteLine("Done.");
}