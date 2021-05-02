using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GiphyApp.Model.Web;

namespace GiphyDotNet.Interfaces
{
    internal interface IApiService
    {
        Task<Result> GetData(Uri uri);
    }
}
