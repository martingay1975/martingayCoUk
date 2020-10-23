using DocXLib.Model.Data.Xml.Info;

namespace DocXLib.Helper.DataToJson
{
    class InfoAssembler
    {
        public Model.Data.Json.Info Copy(Info source)
        {
            if (source == null) return null;

            var destination = new Model.Data.Json.Info {Content = source.Content};
            return destination;
        }
    }
}