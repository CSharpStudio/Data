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
            section.Set("A", "a string");
            section.Set("b", 1);
            section.Set("c", DateTime.Now);
            section.Set("d", 123.123321);
            section.Set("e", new Cfg { Test = "test string", DT = DateTime.Now });
            section.SetList("f", new[] { 1, 2, 3, 5 });
            var child = new JsonConfigSection();
            child.Set("d", 123.123321);
            child.Set("e", new Cfg { Test = "test string", DT = DateTime.Now });
            section.SetSection("g", child);
            var json = section.Save();
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
