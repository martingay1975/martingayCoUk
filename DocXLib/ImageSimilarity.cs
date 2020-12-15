using System;
using System.IO;
using DeepAI; // Add this line to the top of your file
using Newtonsoft.Json;

namespace DocXLib
{
    public class ImageSimilarityResponse
    {
        public int distance { get; set; }
    }

    static class ImageSimilarity
    {
        private static DeepAI_API api;
        public static void Create()
        {
            
        }

        public static int GetSimilarScore(string localFile, string hostFileUrl)
        {
            api = new DeepAI_API(apiKey: "ed6a1b7c-f7ab-428f-997f-474ea0c1e1dc");
            if (api == null)
            {
                throw new InvalidOperationException("The API Ket did not work");
            }

            // Ensure your DeepAI.Client NuGet package is up to date: https://www.nuget.org/packages/DeepAI.Client
            // Example posting a image URL:
            StandardApiResponse resp = api.callStandardApi("image-similarity", new
            {
                image1 = File.OpenRead(localFile),
                image2 = hostFileUrl,
            });

            var re = JsonConvert.DeserializeObject<ImageSimilarityResponse>(api.objectAsJsonString(resp.output));
            return re.distance;
        }
    }
}
