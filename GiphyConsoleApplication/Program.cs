using GiphyApp.Model.Parameters;
using GiphyApp.BL.Tools;
using System;
using System.Threading.Tasks;
using GiphyApp.Model.Results;
using GiphyApp.Model.GiphyImage;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace GiphyConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
           .AddJsonFile(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\appsettings.json")), true, true)
           .Build();
            var apikey = configuration.GetSection("giphyapikey").Value;
            
            bool stayInSearch = true;
            var giphy = new Giphy(apikey,new MemoryCache(new MemoryCacheOptions()));

            while (stayInSearch)
            {
                Console.WriteLine("Please enter a word or words you want to search for gifs for: ");
                var searchQuery = Console.ReadLine();

                if (string.IsNullOrEmpty(searchQuery))
                    continue;
                // Returns gif results
                var gifs = giphy.GetOrCreateSearchResults(searchQuery);
                Console.WriteLine($"Following the search results for '{searchQuery}':\n\n");
                gifs.Wait();
                foreach (var url in gifs.Result)
                {
                    Console.WriteLine(url);

                }

                ConsoleKey response;
                do
                {
                    Console.Write("\n\nAre you want to keep searching for gifs ? [y/n]");
                    response = Console.ReadKey(false).Key;   
                    if (response != ConsoleKey.Enter)
                        Console.WriteLine();

                } while (response != ConsoleKey.Y && response != ConsoleKey.N);

                stayInSearch = response == ConsoleKey.Y;
            }

            Console.WriteLine("\n\nPlease enter any key to get trending gifs\n\n");
            Console.ReadKey();
            Console.WriteLine($"Following the search results for trending gifs:\n\n");
            var trendingParameter = new TrendingParameter();
            var t2 = giphy.TrendingGifs(trendingParameter);
            t2.Wait();
            foreach (Data item in t2.Result.Data)
            {
                Console.WriteLine(item.EmbedUrl);

            }
            Console.ReadKey();

        }
    }
}
