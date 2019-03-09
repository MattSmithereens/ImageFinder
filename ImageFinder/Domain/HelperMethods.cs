﻿using ImageFinder.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageFinder.Domain
{
    public static class HelperMethods
    {
        #region SearchImages
        public static string SearchForImages(string searchTerms)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Master.API_Endpoint + searchTerms);

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
    }
}
