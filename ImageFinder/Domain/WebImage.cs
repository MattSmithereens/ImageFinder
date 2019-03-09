using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFinder.Domain
{
    public class WebImage
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Large_URL { get; set; }
        public string Source_Id { get; set; }
    }
}
