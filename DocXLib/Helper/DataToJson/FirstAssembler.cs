using DocXLib.Model.Data.Xml;

namespace DocXLib.Helper.DataToJson
{
    class FirstAssembler
    {
        public Model.Data.Json.First Copy(First source)
        {
            if (source == null) return null;

            var destination = new Model.Data.Json.First {Name = source.Name, Value = source.Value};
            return destination;
        }
    }
}