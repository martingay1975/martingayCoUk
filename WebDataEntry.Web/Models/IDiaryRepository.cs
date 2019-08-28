using System.Collections.Generic;
using WebDataEntry.Web.Models.Data;

namespace WebDataEntry.Web.Models
{
    public interface IDiaryRepository
    {
        ICollection<Entry> GetEntries(DateEntry dateEntry);
        ICollection<Person> GetPeople();
        ICollection<Location> GetLocations();
        int DeleteEntries(DateEntry dateEntry);
        void Create(Entry entry);
        void Update(UpdateEntryRequest updateEntryRequest);
        void Save();
	    void TestValidDiary();
        void SaveFormats(SaveRequest saveRequest);
        void DownloadDatabase();
        void UploadDatabase();
    }
}