using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ConsoleClient
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            DiscoveryResponse discoveryResponse = await DiscoveryClient.GetAsync("http://localhost:5000");
            //获取令牌
            TokenClient tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, "client", "secret");
            TokenResponse tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            Console.WriteLine($"json:{tokenResponse.Json}");
            Console.WriteLine($"accesstoken:{tokenResponse.AccessToken}");

            using (var client = new HttpClient())
            {
                client.SetBearerToken(tokenResponse.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:5001/identity");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(JArray.Parse(await response.Content.ReadAsStringAsync()));
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                }
            }

            Console.ReadKey();
        }
    }
}
