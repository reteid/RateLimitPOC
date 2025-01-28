using System;
using System.Globalization;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

public class Logger
{
    private readonly RateLimiter _rateLimiter;

    public Logger()
    {
        //configure the ratelimiter options
        var options = new FixedWindowRateLimiterOptions
        {
            PermitLimit = 7, // Allow 7 operations
            Window = TimeSpan.FromSeconds(10), // Within a 10-second window
            AutoReplenishment = true, // Automatically replenish permits
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst // Use a FIFO queue
        };

        _rateLimiter = new FixedWindowRateLimiter(options);
    }

    

    public async Task WriteLogAsync(string message)
    {
        RateLimitLease lease;
        
        //loop until we acquire a lease
        do
        {
            lease = await _rateLimiter.AcquireAsync(1);
            if (!lease.IsAcquired)
            {
                if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    Console.WriteLine($"Rate limit exceeded. Retry after {retryAfter.Seconds.ToString(NumberFormatInfo.InvariantInfo)} seconds");
                }
                await Task.Delay(1000); // Wait for a short period before retrying
            }
        } while (!lease.IsAcquired);

        //execute the log operation
        using (lease)
        {
            string logEntry = $"{DateTime.Now}: {message}{Environment.NewLine}";

            Console.WriteLine(logEntry);
        }        
    }
}
