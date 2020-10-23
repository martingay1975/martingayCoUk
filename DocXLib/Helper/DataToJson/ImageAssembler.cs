using DocXLib.Model.Data.Xml;

namespace DocXLib.Helper.DataToJson
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