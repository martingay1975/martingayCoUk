using WebDataEntry.Web.Models;
using WebDataEntry.Web.Models.Data;

namespace WebDataEntry.Web.Application
{
    public static class AutoMapperSetup
    {
        public static void Setup()
        {
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.Location, Location>();
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.Person, Person>();
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.DateEntry, DateEntry>();
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.Diary, Diary>();
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.Entry, Entry>();
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.First, First>();
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.DiaryImage, DiaryImage>();
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.Info.Info, Info>()
                      .ForMember(destination => destination.Content, opt => opt.MapFrom(srcInfo => srcInfo.OriginalContent));
            AutoMapper.Mapper.CreateMap<DiaryDatabase.Model.Data.Xml.Title, Title>();


            // Setup going the other way around.
            AutoMapper.Mapper.CreateMap<Location, DiaryDatabase.Model.Data.Xml.Location>();
            AutoMapper.Mapper.CreateMap<Person, DiaryDatabase.Model.Data.Xml.Person>();
            AutoMapper.Mapper.CreateMap<DateEntry, DiaryDatabase.Model.Data.Xml.DateEntry>();
            AutoMapper.Mapper.CreateMap<Diary, DiaryDatabase.Model.Data.Xml.Diary>();
            AutoMapper.Mapper.CreateMap<Entry, DiaryDatabase.Model.Data.Xml.Entry>();
            AutoMapper.Mapper.CreateMap<DiaryImage, DiaryDatabase.Model.Data.Xml.DiaryImage>();
            AutoMapper.Mapper.CreateMap<Info, DiaryDatabase.Model.Data.Xml.Info.Info>()
                .ForMember(destination => destination.OriginalContent, opt => opt.MapFrom(srcInfo => srcInfo.Content));
            AutoMapper.Mapper.CreateMap<First, DiaryDatabase.Model.Data.Xml.First>();
            AutoMapper.Mapper.CreateMap<Title, DiaryDatabase.Model.Data.Xml.Title>();

        }
    }
}