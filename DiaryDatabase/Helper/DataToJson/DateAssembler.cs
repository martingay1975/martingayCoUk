using DiaryDatabase.Model.Data.Xml;

namespace DiaryDatabase.Helper.DataToJson
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