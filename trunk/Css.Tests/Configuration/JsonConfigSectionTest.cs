using Css.ComponentModel;
using Css.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Css.Tests.Configuration
{
    public class JsonConfigSectionTest : DisposableBase
    {
        /// <summary>
        /// Save section to json format
        /// </summary>
        [Fact]
        public void SaveTest()
        {
            JsonConfigSection section = new JsonConfigSection();
            section.Set("Key01", "a string");
            section.Set("Key02", 1);
            section.Set("Key03", DateTime.Parse("2017-11-22T23:18:37.4480022+08:00"));
            section.Set("Key04", 123.123321);
            section.Set("Key05", new Cfg { Test = "test string", DT = DateTime.Parse("2017-11-22T23:18:37.4591099+08:00") });
            section.SetList("Key06", new[] { 1, 2, 3, 5 });
            var child = new JsonConfigSection();
            child.Set("Key01", 123.123321);
            child.Set("Key02", new Cfg { Test = "test string", DT = DateTime.Parse("2017-11-22T23:18:37.4707807+08:00") });
            section.SetSection("Key07", child);
            var json = section.Save();
            Assert.Equal(@"{
  ""Key01"": ""a string"",
  ""Key02"": 1,
  ""Key03"": ""2017-11-22T23:18:37.4480022+08:00"",
  ""Key04"": 123.123321,
  ""Key05"": {
    ""Test"": ""test string"",
    ""DT"": ""2017-11-22T23:18:37.4591099+08:00""
  },
  ""Key06"": [
    1,
    2,
    3,
    5
  ],
  ""_sections"": {
    ""Key07"": {
      ""Key01"": 123.123321,
      ""Key02"": {
        ""Test"": ""test string"",
        ""DT"": ""2017-11-22T23:18:37.4707807+08:00""
      }
    }
  }
}", json);
        }

        /// <summary>
        /// Load section from json format
        /// </summary>
        [Fact]
        public void LoadTest()
        {
            var json = @"{
  ""Key01"": ""a string"",
  ""Key02"": 1,
  ""Key03"": ""2017-11-22T23:18:37.4480022+08:00"",
  ""Key04"": 123.123321,
  ""Key05"": {
    ""Test"": ""test string"",
    ""DT"": ""2017-11-22T23:18:37.4591099+08:00""
  },
  ""Key06"": [
    1,
    2,
    3,
    5
  ],
  ""_sections"": {
    ""Key07"": {
      ""Key01"": 123.123321,
      ""Key02"": {
        ""Test"": ""test string"",
        ""DT"": ""2017-11-22T23:18:37.4707807+08:00""
      }
    }
  }
}";
            JsonConfigSection section = JsonConfigSection.Load(json);
            Assert.Equal("a string", section.Get<string>("Key01"));
            Assert.Equal(1, section.Get<int>("Key02"));
            Assert.Equal(DateTime.Parse("2017-11-22T23:18:37.4480022+08:00"), section.Get<DateTime>("Key03"));
            Assert.Equal(123.123321m, section.Get<decimal>("Key04"));
            Assert.Equal("test string", section.Get<Cfg>("Key05").Test);
            Assert.Equal(4, section.GetList<int>("Key06").Count);
            Assert.Equal(123.123321m, section.GetSection("Key07").Get<decimal>("Key01"));
        }
    }

    public class Cfg
    {
        public string Test { get; set; }
        public DateTime DT { get; set; }
    }
}
