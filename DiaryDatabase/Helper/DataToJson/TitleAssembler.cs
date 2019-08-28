using DiaryDatabase.Model.Data.Xml;

namespace DiaryDatabase.Helper.DataToJson
{
    class TitleAssembler
    {
        public Model.Data.Json.Title Copy(Title source)
        {
            if (source == null) return null;

            var destination = new Model.Data.Json.Title {Name = source.Name, Value = source.Value};
            return destination;
        }
    }
}