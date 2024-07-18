using CReed.TimestampGuid;

for (var i = 0; i < 10; i++)
{
    await Task.Delay(100);
    Console.WriteLine(TimestampGuid.NextGuid());
}
