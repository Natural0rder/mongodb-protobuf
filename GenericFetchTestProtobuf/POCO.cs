using System.IO.Compression;
using MongoDB.Bson;
using ProtoBuf;

namespace LoadProtobufSample
{
    public enum NetworkCompression
    {
        Zlib,
        Zstd,
        Snappy
    }

	public abstract class DummyBase
	{
        public BsonObjectId Id { get; set; }
    }

    [ProtoContract]
    public class ContentRaw
    {
        public ContentRaw()
        {
            PropString0 = Guid.NewGuid().ToString();
            PropString1 = Guid.NewGuid().ToString();
            PropString2 = Guid.NewGuid().ToString();
            PropString3 = Guid.NewGuid().ToString();
            PropString4 = Guid.NewGuid().ToString();
            PropString5 = Guid.NewGuid().ToString();
            PropString6 = Guid.NewGuid().ToString();
            PropString7 = Guid.NewGuid().ToString();
            PropString8 = Guid.NewGuid().ToString();
            PropString9 = Guid.NewGuid().ToString();

            var random = new Random();
            PropDouble0 = random.NextDouble();
            PropDouble1 = random.NextDouble();
            PropDouble2 = random.NextDouble();
            PropDouble3 = random.NextDouble();
            PropDouble4 = random.NextDouble();
            PropDouble5 = random.NextDouble();
            PropDouble6 = random.NextDouble();
            PropDouble7 = random.NextDouble();
            PropDouble8 = random.NextDouble();
            PropDouble9 = random.NextDouble();

            var randDates = Helper.GenerateRandomDates(10);

            PropDate0 = randDates.ElementAt(0);
            PropDate1 = randDates.ElementAt(1);
            PropDate2 = randDates.ElementAt(2);
            PropDate3 = randDates.ElementAt(3);
            PropDate4 = randDates.ElementAt(4);
            PropDate5 = randDates.ElementAt(5);
            PropDate6 = randDates.ElementAt(6);
            PropDate7 = randDates.ElementAt(7);
            PropDate8 = randDates.ElementAt(8);
            PropDate9 = randDates.ElementAt(9);

            PropLongString0 = Helper.GenerateRandomString(1000);
            PropLongString1 = Helper.GenerateRandomString(1000);
            PropLongString2 = Helper.GenerateRandomString(1000);
            PropLongString3 = Helper.GenerateRandomString(1000);
            PropLongString4 = Helper.GenerateRandomString(1000);
        }

        [ProtoMember(1)]
        public string PropString0 { get; set; }
        [ProtoMember(2)]
        public string PropString1 { get; set; }
        [ProtoMember(3)]
        public string PropString2 { get; set; }
        [ProtoMember(4)]
        public string PropString3 { get; set; }
        [ProtoMember(5)]
        public string PropString4 { get; set; }
        [ProtoMember(6)]
        public string PropString5 { get; set; }
        [ProtoMember(7)]
        public string PropString6 { get; set; }
        [ProtoMember(8)]
        public string PropString7 { get; set; }
        [ProtoMember(9)]
        public string PropString8 { get; set; }
        [ProtoMember(10)]
        public string PropString9 { get; set; }

        [ProtoMember(11)]
        public double PropDouble0 { get; set; }
        [ProtoMember(12)]
        public double PropDouble1 { get; set; }
        [ProtoMember(13)]
        public double PropDouble2 { get; set; }
        [ProtoMember(14)]
        public double PropDouble3 { get; set; }
        [ProtoMember(15)]
        public double PropDouble4 { get; set; }
        [ProtoMember(16)]
        public double PropDouble5 { get; set; }
        [ProtoMember(17)]
        public double PropDouble6 { get; set; }
        [ProtoMember(18)]
        public double PropDouble7 { get; set; }
        [ProtoMember(19)]
        public double PropDouble8 { get; set; }
        [ProtoMember(20)]
        public double PropDouble9 { get; set; }

        [ProtoMember(21)]
        public DateTime PropDate0 { get; set; }
        [ProtoMember(22)]
        public DateTime PropDate1 { get; set; }
        [ProtoMember(23)]
        public DateTime PropDate2 { get; set; }
        [ProtoMember(24)]
        public DateTime PropDate3 { get; set; }
        [ProtoMember(25)]
        public DateTime PropDate4 { get; set; }
        [ProtoMember(26)]
        public DateTime PropDate5 { get; set; }
        [ProtoMember(27)]
        public DateTime PropDate6 { get; set; }
        [ProtoMember(28)]
        public DateTime PropDate7 { get; set; }
        [ProtoMember(29)]
        public DateTime PropDate8 { get; set; }
        [ProtoMember(30)]
        public DateTime PropDate9 { get; set; }

        [ProtoMember(31)]
        public string PropLongString0 { get; set; }
        [ProtoMember(32)]
        public string PropLongString1 { get; set; }
        [ProtoMember(33)]
        public string PropLongString2 { get; set; }
        [ProtoMember(34)]
        public string PropLongString3 { get; set; }
        [ProtoMember(35)]
        public string PropLongString4 { get; set; }
    }

    public class DummyForBson : DummyBase
    {
        public ContentRaw Content { get; set; } = new ContentRaw();
    }

    public class DummyForProtobuf : DummyBase
    {
        public DummyForProtobuf(bool zip = false)
        {
            byte[] content;

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, new ContentRaw());
                content = stream.ToArray();
            }

            if (!zip)
            {
                Content = content;
                return;
            }
            
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(content, 0, content.Length);
                zipStream.Close();
                Content = compressedStream.ToArray();
            }
        }

        public byte[] Content { get; set; }
    }
}
