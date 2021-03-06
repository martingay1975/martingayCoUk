﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using DocXLib.Helper.DataToJson;
using DocXLib.Model.Data.Xml;

namespace DocXLib.Helper
{
    public class JsonFileSerialize : IFileStreamSerialize
    {
        public string Filename { get; set; }

        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(Model.Data.Json.Diary));

		public bool CopyOptions { get; set; }

        public int Serialize(StreamWriter stream, Diary diary)
        {
            var jsonDiary = DiaryAssembler.Instance.Copy(diary, CopyOptions);
            JsonSerializer.WriteObject(stream.BaseStream, jsonDiary);
	        return jsonDiary.Hash;
        }

        public Diary Deserialize(StreamReader stream)
        {
            throw new NotSupportedException();
        }
    }
}