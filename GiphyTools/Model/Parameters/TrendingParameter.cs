using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiphyApp.Model.Parameters
{
    public class TrendingParameter
    {
        public int Limit { get; set; } = 25;

        public Rating Rating { get; set; }

    }
}
