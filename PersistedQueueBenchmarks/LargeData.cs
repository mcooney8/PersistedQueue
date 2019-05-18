using System;
using System.Collections.Generic;
using System.Text;

namespace PersistedQueueBenchmarks
{
    [Serializable]
    public class LargeData
    {
        public LargeData()
        {
            Random random = new Random();
            DoubleValue = random.NextDouble();
            byte[] buffer = new byte[1024];
            random.NextBytes(buffer);
            StringValue1 = Encoding.ASCII.GetString(buffer);
            random.NextBytes(buffer);
            StringValue2 = Encoding.ASCII.GetString(buffer);
            random.NextBytes(buffer);
            StringValue3 = Encoding.ASCII.GetString(buffer);
            BoolValue = random.Next() % 2 == 0;
        }
        public double DoubleValue { get; }
        public string StringValue1 { get; }
        public string StringValue2 { get; }
        public string StringValue3 { get; }
        public bool BoolValue { get; }
    }
}
