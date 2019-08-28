namespace WebDataEntry.Web.Models
{
    public interface IDiaryAssembler
    {
        Diary Map(DiaryDatabase.Model.Data.Xml.Diary source);
        DiaryDatabase.Model.Data.Xml.Diary Map(Diary source);
    }
}