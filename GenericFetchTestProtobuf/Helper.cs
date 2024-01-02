using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using ProtoBuf;

namespace LoadProtobufSample
{
    public static class Helper
	{
        public static ContentRaw FromProtobuf(this byte[] content, bool unzip = false)
        {  
            if (unzip)
            {
                byte[] unzippedContent;

                using (var compressedStream = new MemoryStream(content))
                using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                using (var resultStream = new MemoryStream())
                {
                    zipStream.CopyTo(resultStream);
                    unzippedContent = resultStream.ToArray();
                }

                using (var stream = new MemoryStream(unzippedContent))
                {
                    return Serializer.Deserialize<ContentRaw>(stream);
                }
            }

            using (var stream = new MemoryStream(content))
            {
                return Serializer.Deserialize<ContentRaw>(stream);
            }
        }

        public static IEnumerable<DateTime> GenerateRandomDates(int numberOfDates)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            for (int i = 0; i < numberOfDates; i++)
            {
                var year = rnd.Next(1, 10000);
                var month = rnd.Next(1, 13);
                var days = rnd.Next(1, DateTime.DaysInMonth(year, month) + 1);

                yield return new DateTime(year, month, days,
                    rnd.Next(0, 24), rnd.Next(0, 60), rnd.Next(0, 60), rnd.Next(0, 1000));
            }
        }

        public static string GenerateRandomString(int size, bool lowerCase = false)
        {
            var random = new Random();
            var builder = new StringBuilder();

            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26

            for (var i = 0; i < size; i++)
            {
                var @char = (char)random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }
}

