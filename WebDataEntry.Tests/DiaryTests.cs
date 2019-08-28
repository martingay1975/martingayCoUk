using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiaryDatabase.Model.Data.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebDataEntry.Tests
{
    [TestClass]
    public class DiaryTests
    {
        [TestMethod]
        public void LoadOriginalDiaryXml()
        {
            var testDiaryXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\diary.xml");
            if (!File.Exists(testDiaryXmlPath))
                throw new FileNotFoundException("Unable to find test xml file", testDiaryXmlPath);

            var diaryHelper = new DiaryDatabase.Presenter.DiarySerialization();
            var originalDiary = diaryHelper.LoadXml(testDiaryXmlPath);

            // test on original data
            AssertionsOnDiary(originalDiary);

            // save the data to another file
            var targetDiaryXmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\diaryPostProcessing.xml");
            diaryHelper.SaveAndValidateXml(targetDiaryXmlPath, originalDiary);

            // load that data back into a diary object
            var firstParseDiary = diaryHelper.LoadXml(targetDiaryXmlPath);

            AssertionsOnDiary(firstParseDiary);

            Assert.AreEqual(originalDiary.Entries.Count, firstParseDiary.Entries.Count);
            Assert.AreEqual(originalDiary.Locations.Count, firstParseDiary.Locations.Count);
            Assert.AreEqual(originalDiary.People.Count, firstParseDiary.People.Count);

            // CRUD operations
            AmendGreenwichEntry(firstParseDiary);
            AddEntry(firstParseDiary);
            RemoveEntry(firstParseDiary);

            var targetDiaryXmlPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\diaryPostProcessing2.xml");
			diaryHelper.SaveAndValidateXml(targetDiaryXmlPath2, firstParseDiary);

            var postCruDoperationsDiary = diaryHelper.LoadXml(targetDiaryXmlPath2);
            AssertionsOnDiary(postCruDoperationsDiary);

            // Test whether CRUD operation persisted and loaded ok
            TestEntryAdded(postCruDoperationsDiary);
            TestAmendGreenwichEntry(postCruDoperationsDiary);
            TestRemoveEntry(postCruDoperationsDiary);
        }

        private void AssertionsOnDiary(Diary diary)
        {
            Assert.IsTrue(diary.Entries.Count > 2000);
            Assert.IsTrue(diary.Locations.Count > 100);
            Assert.IsTrue(diary.People.Count > 50);

            TestFirstTrainRideEntry(diary);
            Test8May2010(diary);
        }


        private void TestEntryAdded(Diary diary)
        {
            diary.Entries.First(entry => entry.DateEntry.Year == 2028 &&
                                         entry.DateEntry.Month == 8 &&
                                         entry.DateEntry.Day == 5 &&
                                         entry.Title.Value == "Ben's 21st Birthday" &&
                                         entry.People.Contains(1) &&
                                         entry.People.Contains(2) &&
                                         entry.Locations.Contains(3) &&
                                         entry.Locations.Contains(4) &&
                                         entry.Locations.Contains(5) &&
                                         entry.Locations.Count == 3 &&
                                         entry.People.Count == 2);
        }

        private void AddEntry(Diary diary)
        {
            var entry = new Entry
                            {
                                DateEntry = new DateEntry {Year = 2028, Month = 8, Day = 5},
                                Title = new Title() {Value = "Ben's 21st Birthday"},
                                People = new List<int>() {1, 2},
                                Locations = new List<int>() {3, 4, 5}
                            };

            diary.Entries.Add(entry);
        }

        private void RemoveEntry(Diary diary)
        {
            var entryDeleted = diary.Entries.First(entry => entry.Title.Value == "Denmark Holiday - Lise's 50th");
            diary.Entries.Remove(entryDeleted);
        }

        private void TestRemoveEntry(Diary diary)
        {
            var entryDeleted = diary.Entries.FirstOrDefault(entry => entry.Title != null && entry.Title.Value != null && entry.Title.Value == "Denmark Holiday - Lise's 50th");
            Assert.IsNull(entryDeleted);
        }


        private void TestAmendGreenwichEntry(Diary diary)
        {
            var greenwichEntry = GetGreenwichEntry(diary);
            Assert.IsTrue(greenwichEntry.Locations.Contains(54));
            Assert.IsTrue(greenwichEntry.People.Contains(2));
            Assert.AreEqual("<p>This is a test. Lise and Jørgen</p>", greenwichEntry.Info.OriginalContent);
        }

        private void AmendGreenwichEntry(Diary diary)
        {
            var greenwichEntry = GetGreenwichEntry(diary);
            greenwichEntry.Locations.Add(54);
            greenwichEntry.People.Add(2);
            greenwichEntry.Info.OriginalContent = "<p>This is a test. Lise and Jørgen</p>";
        }

        private Entry GetGreenwichEntry(Diary diary)
        {
            return diary.Entries.FirstOrDefault(entry =>
                                            entry.DateEntry.Year == 1997 &&
                                            entry.DateEntry.Month == 8 &&
                                            entry.Title.Value == "Greenwich");

        }

        private void TestFirstTrainRideEntry(Diary diary)
        {
            Enumerable.First(diary.Entries, entry => 
                entry.DateEntry.Year == 1975 && 
                entry.DateEntry.Month == 10 && 
                entry.Title.Value == "First Train Ride" &&
                entry.First.Value == "First Train Ride" && 
                entry.First.Name == "martin");
        }

        private void Test8May2010(Diary diary)
        {
            Enumerable.First(diary.Entries, entry =>
                entry.DateEntry.Year == 2010 &&
                entry.DateEntry.Month == 5 &&
                entry.DateEntry.Day == 8 &&
                entry.Title.Value == "Skate Jam, Christian Aid collecting, Swimming and Come Dine With Me" &&
                entry.First.Value == "Lost first tooth, middle, bottom." &&
                entry.First.Name == "katie" && 
                entry.People.Count == 10 &&
                entry.People.Contains(202));
        }
    }
}
