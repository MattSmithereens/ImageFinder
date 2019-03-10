using ImageFinder.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media.Imaging;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Drawing;
using System.Windows.Media;

namespace ImageFinder.Domain
{
    public static class HelperMethods
    {
        #region SearchImages
        public static string SearchForImages(string searchTerms)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ImageFinder.Config.Master.API_Endpoint + searchTerms);

            try
            {
                WebResponse response = request.GetResponse();

                using(Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);

                    return reader.ReadToEnd();
                }
            }
            catch(WebException ex)
            {
                WebResponse errorResponse = ex.Response;

                using(Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();

                    //TODO: create error log

                    return "Failed";
                }
            }
        }
        #endregion

        #region ParseSearchTerms
        public static string ParsedSearchTerms(List<string> searchTerms)
        {
            string searchString = string.Empty;

            foreach(var word in searchTerms)
            {
                if (!searchString.Contains(word))
                    searchString += word;

                if (word != searchTerms[searchTerms.Count - 1])
                    searchString += ", ";
            }

            return searchString;
        }
        #endregion

        public static IEnumerable<WebImage> GetImagesFromJson(string json)
        {
            var jsonArray = json.Substring(json.IndexOf("["));
            jsonArray = jsonArray.Substring(0, jsonArray.Length - 1);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WebImage[] images = serializer.Deserialize<WebImage[]>(jsonArray);

            return images.Reverse();
        }

        public static void ExportSlide(string title, string body, BitmapImage img)
        {
            Application pptApplication = new Application();
            string fileName = @"imageFinderResult";

            fileName += img.UriSource.AbsoluteUri.Substring(img.UriSource.AbsoluteUri.LastIndexOf("."));

            Presentation pptPresentation = pptApplication.Presentations.Add(MsoTriState.msoTrue);

            Microsoft.Office.Interop.PowerPoint.Slides slides;
            Microsoft.Office.Interop.PowerPoint._Slide slide;
            Microsoft.Office.Interop.PowerPoint.TextRange objText;
            Microsoft.Office.Interop.PowerPoint.TextRange objTextBody;
            Microsoft.Office.Interop.PowerPoint.CustomLayout custLayout = pptPresentation.SlideMaster.CustomLayouts[Microsoft.Office.Interop.PowerPoint.PpSlideLayout.ppLayoutText];

            slides = pptPresentation.Slides;
            slide = slides.AddSlide(1, custLayout);

            objText = slide.Shapes[1].TextFrame.TextRange;
            objText.Text = title;
            objText.Font.Name = "Arial";
            objText.Font.Size = 40;

            Microsoft.Office.Interop.PowerPoint.Shape shape = slide.Shapes[2];

            byte[] data;
            using (WebClient client = new WebClient())
            {
                data = client.DownloadData(img.UriSource.AbsoluteUri);
            }
            File.WriteAllBytes(fileName, data);
            FileInfo file = new FileInfo(fileName);

            slide.Shapes.AddPicture(file.FullName, MsoTriState.msoFalse, MsoTriState.msoTrue, shape.Left, shape.Top, shape.Width, shape.Height);

            objTextBody = slide.Shapes[1].TextFrame.TextRange;
            objTextBody.Text = body;
            objTextBody.Font.Name = "Arial";
            objTextBody.Font.Size = 32;

            pptPresentation.SaveAs(@"ImageFinderSlide.pptx", Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoTrue);
        }
    }
}
