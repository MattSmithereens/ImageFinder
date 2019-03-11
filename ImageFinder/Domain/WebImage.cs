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
        public string PageURL { get; set; }
        public string Type { get; set; }
        public int PreviewWidth { get; set; }
        public int PreviewHeight { get; set; }
        public string WebFormatURL { get; set; }
        public int WebFormatWidth { get; set; }
        public int WebFormatHeight { get; set; }
        public string LargeImageURL { get; set; }
        public string FullHDURL { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int ImageSize { get; set; }
        public int Views { get; set; }
        public int Downloads { get; set; }
        public int Favorites { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
        public int User_ID { get; set; }
        public string User { get; set; }
        public string UserImageURL { get; set; }
    }
}
