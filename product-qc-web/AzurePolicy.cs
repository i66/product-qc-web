using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace product_qc_web
{
    public class AzurePolicy : PolicyBase
    {
        public override void Run(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder().
                UseStartup<Startup>().Start();
            builder.Run();
        }
    }
}
