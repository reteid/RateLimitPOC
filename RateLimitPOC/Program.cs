using System;

namespace RateLimitPOC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger();

            try
            {                
                for (int i = 1; i <= 13; i++)
                {
                    
                   logger.WriteLogAsync($"Hello, World! {i}").Wait();
                }

                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            
        }
    }
}
