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
using GemBox.Presentation;
using System.Diagnostics;

namespace ImageFinder.Domain
{
    public static class HelperMethods
    {
        #region SearchImages
        public static string SearchForImages(string searchTerms)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ImageFinder.Config.Master.API_Endpoint + searchTerms + "&image_type=photo&per_page=50");

            request.ServerCertificateValidationCallback = delegate { return true; };

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
                    searchString += "+";
            }

            return searchString;
        }
        #endregion

        public static IEnumerable<WebImage> GetImagesFromJson(string json)
        {
            var jsonArray = json.Substring(json.IndexOf("["));
            jsonArray = jsonArray.Substring(0, jsonArray.LastIndexOf("]") + 1);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WebImage[] images = serializer.Deserialize<WebImage[]>(jsonArray);

            return images;
        }

        public static void ExportSlide(string title, string body, BitmapImage[] imgs)
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            string[] fileNames = GetFileNames(imgs);
            FileInfo[] files = DownloadFiles(imgs, fileNames);

            var presentation = new PresentationDocument();

            var slide = presentation.Slides.AddNew(SlideLayoutType.Custom);

            var titleTxtBox = slide.Content.AddTextBox(15, 2, 5, 4, LengthUnit.Centimeter);
            titleTxtBox.Format.Centered = true;

            var bodyTxtBox = slide.Content.AddTextBox(15, 4, 5, 4, LengthUnit.Centimeter);
            bodyTxtBox.Format.Centered = true;

            var paragraph = titleTxtBox.AddParagraph();
            var paragraph2 = bodyTxtBox.AddParagraph();

            paragraph.AddRun(title);
            paragraph.Format.Character.Size = 40;

            paragraph2.AddRun(body);
            paragraph2.Format.Character.Size = 32;

            for(int i = 0; i < imgs.Length; i++)
            {
                var picture = slide.Content.AddPicture(files[i].FullName, 2 * (i * 5) + 5, 12, 6, 5, LengthUnit.Centimeter);
            }

            presentation.Save("ImageFinderSlide.pptx");

            FileInfo ppt = new FileInfo("ImageFinderSlide.pptx");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "POWERPNT.exe";
            psi.Arguments = ppt.FullName;
            Process.Start(psi);
        }

        private static string[] GetFileNames(BitmapImage[] imgs)
        {
            List<string> fileNames = new List<string>() { };

            for(int i = 0; i < imgs.Length; i++)
            {
                fileNames.Add($"image{i + 1}{imgs[i].UriSource.AbsoluteUri.Substring(imgs[i].UriSource.AbsoluteUri.LastIndexOf('.'))}");
            }

            return fileNames.ToArray();
        } 

        private static FileInfo[] DownloadFiles(BitmapImage[] imgs, string[] fileNames)
        {
            List<FileInfo> files = new List<FileInfo>() { };

            for(int i = 0; i < imgs.Length; i++)
            {
                byte[] data;
                using (WebClient client = new WebClient())
                {
                    data = client.DownloadData(imgs[i].UriSource.AbsoluteUri);
                }
                File.WriteAllBytes(fileNames[i], data);
                FileInfo file = new FileInfo(fileNames[i]);

                files.Add(file);
            }

            return files.ToArray();
        }
    }
}
