using System;

namespace DiaryDatabase.Model.Data.Xml
{
    [Serializable()]
    public class DiaryImage
    {
        public DiaryImage()
        {
            
        }

        public DiaryImage(string path, string description)
        {
            Path = path;
            Description = description;
        }

        public string Path { get; set; }
        public string Description { get; set; }
    }
}