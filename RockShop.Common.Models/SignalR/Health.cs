namespace RockShop.Common.Models.SignalR;

public record Health
{
    public string ServerName { get; set; }
    public double CpuUsageRate { get; set; }
    public double MemoryRate { get; set; }

    public override string ToString()
    {
        return $"{ServerName}, CPU ({CpuUsageRate}%), Mem ({MemoryRate}&)";
    }
}