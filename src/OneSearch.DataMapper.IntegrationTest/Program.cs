using System.Threading.Tasks;

namespace OneSearch.DataMapper.IntegrationTest
{
    class Program
    {
        private static async Task<int> Main(string[] args)
        {
            return await new Startup().RunAsync<DataMapperApp>();
        }
    }
}