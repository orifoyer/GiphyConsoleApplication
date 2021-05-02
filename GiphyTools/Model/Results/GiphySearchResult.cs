using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiphyApp.Model.GiphyImage;
using Newtonsoft.Json;

namespace GiphyApp.Model.Results
{
    public class GiphySearchResult
    {
        [JsonProperty("data")]
        public Data[] Data { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
