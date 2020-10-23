using DocXLib.Model.Data.Xml;

namespace DocXLib.Helper.DataToJson
{
    class DateAssembler
    {
        public Model.Data.Json.Date Copy(DateEntry source)
        {
            var destination = new Model.Data.Json.Date
                                  {
                                      Day = source.Day,
                                      //DayOfWeek = source.DayOfWeek,
                                      Month = source.Month,
                                      Year = source.Year
                                  };
            return destination;
        }
    }
}