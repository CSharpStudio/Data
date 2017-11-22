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
    public class JsonConfigTest : DisposableBase
    {
        [Fact]
        public void GetSetValue()
        {
            ShowProperty("{}");
            ShowProperty("{'Name':'abc'}");
            ShowProperty("{'Name':'abc', 'Qty':0}");
            ShowProperty("{'Array':[1,2,3]}");
            ShowProperty("{'Array':[{'Name':'1'},{'Name':'2'}]}");
        }

        [Fact]
        public void SaveTest()
        {
            JsonConfigSection section = new JsonConfigSection();
            section.Set("Key01", "a string");
            section.Set("Key02", 1);
            section.Set("Key03", DateTime.Now);
            section.Set("Key04", 123.123321);
            section.Set("Key05", new Cfg { Test = "test string", DT = DateTime.Now });
            section.SetList("Key06", new[] { 1, 2, 3, 5 });
            var child = new JsonConfigSection();
            child.Set("Key01", 123.123321);
            child.Set("Key02", new Cfg { Test = "test string", DT = DateTime.Now });
            section.SetSection("Key07", child);
            var json = section.Save();
        }

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
            JsonConfigSection section = new JsonConfigSection();
            section.Load(json);
            Assert.Equal("a string", section.Get<string>("Key01"));
            Assert.Equal(1, section.Get<int>("Key02"));
            Assert.Equal(DateTime.Parse("2017-11-22T23:18:37.4480022+08:00"), section.Get<DateTime>("Key03"));
            Assert.Equal(123.123321m, section.Get<decimal>("Key04"));
            Assert.Equal("test string", section.Get<Cfg>("Key05").Test);
        }

        class Cfg
        {
            public string Test { get; set; }
            public DateTime DT { get; set; }
        }

        void ShowProperty(string json)
        {
            var obj = JObject.Parse(json);
            ShowProperty(obj);
        }

        void ShowProperty(JToken token)
        {
            if (token.Type == JTokenType.Array)
            {
                ShowProperty((JArray)token);
            }
            else if (token.Type == JTokenType.Object)
            {
                ShowProperty((JObject)token);
            }
            else if (token.Type == JTokenType.Property)
            {
                ShowProperty((JProperty)token);
            }
            else
            {
                Debug.WriteLine("{0}".FormatArgs(token));
            }
        }

        void ShowProperty(JProperty property)
        {
            Debug.Write("{0}:".FormatArgs(property.Name));
            ShowProperty(property.Value);
        }

        void ShowProperty(JObject obj)
        {
            foreach (var p in obj.Properties())
            {
                ShowProperty(p);
            }
        }
        void ShowProperty(JArray array)
        {
            foreach (var p in array.Values())
            {
                ShowProperty(p);
            }
        }
    }
}
