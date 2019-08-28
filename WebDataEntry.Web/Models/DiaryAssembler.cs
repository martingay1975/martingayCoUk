namespace WebDataEntry.Web.Models
{
    public class DiaryAssembler : IDiaryAssembler
    {
        /// <summary>
        /// Maps from the model Diary to the one defined in this assembly
        /// </summary>
        /// <param name="source">The Model version of the diary</param>
        /// <returns>Locally defined diary</returns>
        public Diary Map(DiaryDatabase.Model.Data.Xml.Diary source)
        {
            return AutoMapper.Mapper.Map<DiaryDatabase.Model.Data.Xml.Diary, Diary>(source);
        }

        /// <summary>
        /// Maps from Diary defined in this assembly to the one defined in the Model
        /// </summary>
        /// <param name="source">Locally defined diary</param>
        /// <returns>The Model version of the diary</returns>
        public DiaryDatabase.Model.Data.Xml.Diary Map(Diary source)
        {
            return AutoMapper.Mapper.Map<Diary, DiaryDatabase.Model.Data.Xml.Diary>(source);
        }

    }
}