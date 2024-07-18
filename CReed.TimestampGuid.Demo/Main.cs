using CReed.TimestampGuid;

for (var i = 0; i < 10; i++)
{
    await Task.Delay(100);
    var guid = TimestampGuid.NextGuid();
    var timestamp = TimestampGuid.GetTimestamp(guid).ToLocalTime();
    Console.WriteLine($"{guid} {timestamp:O}");
}
