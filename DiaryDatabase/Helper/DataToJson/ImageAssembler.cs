using DiaryDatabase.Model.Data.Xml;

namespace DiaryDatabase.Helper.DataToJson
{
    class ImageAssembler
    {
        public Model.Data.Json.Image Copy(DiaryImage source)
        {
            var destination = new Model.Data.Json.Image()
                                  {
                                      Description = source.Description,
                                      Source = source.Path
                                  };
            return destination;
        }
    }
}