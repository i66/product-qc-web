namespace product_qc_web
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if AZURE_POLICY
            PolicyBase policy = new AzurePolicy();
#else
            PolicyBase policy = new ServiceOrConsolePolicy();
#endif
            policy.Run(args);
        }


    }
}
