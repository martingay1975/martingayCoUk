using System.Collections.Generic;

namespace DiaryDatabase.Model.Data.Xml.Info
{
    public class Whoop : IImageConsumer
    {
        public string Title { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public List<DiaryImage> Images { get; private set; }
        public void AddImage(string path, string description)
        {
            if (Images == null)
                Images = new List<DiaryImage>();

            Images.Add(new DiaryImage(path, description));
        }
    }
}