using System.Diagnostics;
using LoadProtobufSample;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

const string DB_NAME = "protobuf";
const string COLL_NAME = "dummy-bson";
const string COLL_NAME_PB = "dummy-protobuf";
const string COLL_NAME_ZIP_PB = "dummy-protobuf-zipped";
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
    PrepareDataset(connectionString);

Run(connectionString, fetchCountPerRun, NetworkCompression.Snappy);
Run(connectionString, fetchCountPerRun, NetworkCompression.Zlib);
Run(connectionString, fetchCountPerRun, NetworkCompression.Zstd);
Run(connectionString, fetchCountPerRun);

Console.ReadLine();

void RunFetch<T>(IMongoCollection<T> coll, bool unzip = false)
{
    var sw = new Stopwatch();
    sw.Start();

    // Build your query here
    var docs = coll.Find(Builders<T>.Filter.Empty).ToList();

    var elapsedFetch = sw.ElapsedMilliseconds;
    Console.WriteLine($"{elapsedFetch} ms to fetch {docs.Count()} documents.");

    if (typeof(T) == typeof(DummyForProtobuf))
    {
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
        Console.WriteLine($"{elapsedDeserialize} ms to deserialize (unzip = {unzip}) {docs.Count()} documents.");
    }
}

void Run(string cs, int fetchCount, NetworkCompression? networkCompression = null)
{
    switch (networkCompression)
    {
        case NetworkCompression.Zlib:
            cs = cs + "/?compressors=zlib";
            Console.WriteLine("Run with -Zlib- network compression...");
            break;
        case NetworkCompression.Zstd:
            cs = cs + "/?compressors=zstd";
            Console.WriteLine("Run with -Zstd- network compression...");
            break;
        case NetworkCompression.Snappy:
            cs = cs + "/?compressors=snappy";
            Console.WriteLine("Run with -Snappy- network compression...");
            break;
        default:
            Console.WriteLine("Run with -No- network compression...");
            break;
    }

    var client = new MongoClient(cs);
    var database = client.GetDatabase(DB_NAME);
    Console.WriteLine($"Start {fetchCount} run(s) for BSON...");
    for (int i = 0; i < fetchCount; i++)
        RunFetch(database.GetCollection<DummyForBson>(COLL_NAME));
    Console.WriteLine($"Start {fetchCount} run(s) for Protobuf...");
    for (int i = 0; i < fetchCount; i++)
        RunFetch(database.GetCollection<DummyForProtobuf>(COLL_NAME_PB));
    Console.WriteLine($"Start {fetchCount} run(s) for GZipped Protobuf...");
    for (int i = 0; i < fetchCount; i++)
        RunFetch(database.GetCollection<DummyForProtobuf>(COLL_NAME_ZIP_PB), true);
    Console.WriteLine("Done.");
}

void PrepareDataset(string cs)
{
    var client = new MongoClient(cs);
    var database = client.GetDatabase(DB_NAME);
    var collectionBson = database.GetCollection<DummyForBson>(COLL_NAME);
    var collectionProtobuf = database.GetCollection<DummyForProtobuf>(COLL_NAME_PB);
    var collectionProtobufZipped = database.GetCollection<DummyForProtobuf>(COLL_NAME_ZIP_PB);
    Console.WriteLine("Preparing data set...");

    var dummyBson = new List<DummyForBson>();

    for (int i = 0; i < 10000; i++)
    {
        dummyBson.Add(new DummyForBson());
    }

    var dummyProtobuf = new List<DummyForProtobuf>();

    for (int i = 0; i < 10000; i++)
    {
        dummyProtobuf.Add(new DummyForProtobuf());
    }

    var dummyProtobufZipped = new List<DummyForProtobuf>();

    for (int i = 0; i < 10000; i++)
    {
        dummyProtobufZipped.Add(new DummyForProtobuf(true));
    }

    Console.WriteLine("Pushing Bson...");
    database.DropCollection(COLL_NAME);
    collectionBson.InsertMany(dummyBson);

    Console.WriteLine("Pushing Protobuf...");
    database.DropCollection(COLL_NAME_PB);
    collectionProtobuf.InsertMany(dummyProtobuf);

    Console.WriteLine("Pushing Gzipped Protobuf...");
    database.DropCollection(COLL_NAME_ZIP_PB);
    collectionProtobufZipped.InsertMany(dummyProtobufZipped);

    Console.WriteLine("Done.");
}