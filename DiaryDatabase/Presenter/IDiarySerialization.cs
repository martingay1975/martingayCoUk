using DiaryDatabase.Model.Data.Xml;

namespace DiaryDatabase.Presenter
{
    public interface IDiarySerialization
    {
        Diary LoadXml(string filename);
        void SaveJson(string filename);
		int SaveJsonDiary(string filePath, Diary diary, bool copyOptions);
		void SaveAndValidateXml(string outpath, Diary diary);
	    void TestValidDiary(string outpath, Diary diary);
    }
}