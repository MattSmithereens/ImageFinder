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
        public string ImageURL { get; set; }
        public string Tags { get; set; }
        public string PreviewURL { get; set; }
    }
}
