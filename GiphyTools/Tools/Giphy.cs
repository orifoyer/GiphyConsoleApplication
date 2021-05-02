using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using GiphyDotNet.Interfaces;
using GiphyApp.Model.Parameters;
using GiphyApp.Model.Results;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using GiphyApp.Model.GiphyImage;

namespace GiphyApp.BL.Tools
{
    public class Giphy
    {
        private readonly IApiService _apiService = new ApiService();
        private IMemoryCache _cache;
        private ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
        private readonly string _authKey;

        private const string BaseUrl = "https://api.giphy.com/";
        private const string BaseGif = "v1/gifs";
        private const string BaseSticker = "v1/stickers";

        public Giphy(string authKey,IMemoryCache cache)
        {
            _authKey = authKey;
            _cache = cache;
        }

        public async Task<GiphySearchResult> GifSearch(SearchParameter searchParameter)
        {
            if (string.IsNullOrEmpty(searchParameter.Query))
            {
                throw new FormatException("Must set query in order to search.");
            }

            var nvc = new NameValueCollection();
            nvc.Add("api_key", _authKey);
            nvc.Add("q", searchParameter.Query);
            nvc.Add("limit", searchParameter.Limit.ToString());
            nvc.Add("offset", searchParameter.Offset.ToString());
            if(searchParameter.Rating != Rating.None)
                nvc.Add("rating", searchParameter.Rating.ToFriendlyString());

            var result =
                await _apiService.GetData(new Uri($"{BaseUrl}{BaseGif}/search{UriExtensions.ToQueryString(nvc)}"));
            if (!result.IsSuccess)
            {
                throw new WebException($"Failed to get gifs: {result.ResultJson}");
            }

            return JsonConvert.DeserializeObject<GiphySearchResult>(result.ResultJson);
        }

        public async Task<GiphySearchResult> TrendingGifs(TrendingParameter trendingParameter)
        {
            var nvc = new NameValueCollection();
            nvc.Add("api_key", _authKey);
            nvc.Add("limit", trendingParameter.Limit.ToString());
            if (trendingParameter.Rating != Rating.None)
                nvc.Add("rating", trendingParameter.Rating.ToFriendlyString());
            var result =
                await _apiService.GetData(new Uri($"{BaseUrl}{BaseGif}/trending{UriExtensions.ToQueryString(nvc)}"));
            if (!result.IsSuccess)
            {
                throw new WebException($"Failed to get gifs: {result.ResultJson}");
            }

            return JsonConvert.DeserializeObject<GiphySearchResult>(result.ResultJson);
        }

        public async Task<List<string>> GetOrCreateSearchResults(string key)
        {
            List<string> cacheEntry;
            key = key.ToLower();
            if (!_cache.TryGetValue(key.ToLower(), out cacheEntry))// Look for cache key.
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));

                await mylock.WaitAsync();
                try
                {
                    if (!_cache.TryGetValue(key, out cacheEntry))
                    {
                        cacheEntry = new List<string>();
                        // Key not in cache, so get data.
                        var searchParameter = new SearchParameter()
                        {
                            Query = key
                        };
                        var searchTask = await GifSearch(searchParameter);
                        List<Data> lst = new List<Data>(searchTask.Data);
                        Parallel.ForEach(searchTask.Data, item =>
                        {
                            cacheEntry.Add(item.EmbedUrl);
                        });
                        _cache.Set(key, cacheEntry, DateTime.Now.AddHours(1.0));
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            return cacheEntry;
        }
    }

}
