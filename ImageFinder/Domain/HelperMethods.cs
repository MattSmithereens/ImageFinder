using ImageFinder.Config;
using System;
using System.Net.Http;

namespace ImageFinder.Domain
{
    public static class HelperMethods
    {
        public static void SearchForImages(string searchTerms)
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Master.API_Endpoint);

                var response = client.GetAsync(searchTerms);
                response.Wait();

                var result = response.Result;

                if(result.IsSuccessStatusCode)
                {
                    var image = result.Content.ReadAsAsync<Student[]>();
                    
                }
            }
        }
    }
}
