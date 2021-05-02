using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiphyApp.Model.Parameters
{
    public class SearchParameter
    {
        public string Query { get; set; }

        public int Limit { get; set; } = 25;

        public int Offset { get; set; } = 0;

        public Rating Rating { get; set; }
    }
}
