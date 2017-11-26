using Css.ComponentModel;
using Css.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace Css.Tests.Configuration
{
    public class XmlConfigSectionTest : DisposableBase
    {
        /// <summary>
        /// Save section to xml format
        /// </summary>
        [Fact]
        public void SaveTest()
        {
            XmlConfigSection section = new XmlConfigSection();
            section.Set("Key01", "a string");
            section.Set("Key02", 1);
            section.Set("Key03", DateTime.Parse("2017-11-22T23:18:37.4480022+08:00"));
            section.Set("Key04", 123.123321);
            section.Set("Key05", new Cfg { Test = "test string", DT = DateTime.Parse("2017-11-22T23:18:37.4591099+08:00") });
            section.SetList("Key06", new[] { 1, 2, 3, 5 });
            var child = new XmlConfigSection();
            child.Set("Key01", 123.123321);
            child.Set("Key02", new Cfg { Test = "test string", DT = DateTime.Parse("2017-11-22T23:18:37.4707807+08:00") });
            section.SetSection("Key07", child);
            var json = section.Save();
            Assert.Equal(@"<Sections>
  <Section key=""Key01"">a string</Section>
  <Section key=""Key02"">1</Section>
  <Section key=""Key03"">11/22/2017 23:18:37</Section>
  <Section key=""Key04"">123.123321</Section>
  <Object key=""Key05"">&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
&lt;Cfg xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
  &lt;Test&gt;test string&lt;/Test&gt;
  &lt;DT&gt;2017-11-22T23:18:37.4591099+08:00&lt;/DT&gt;
&lt;/Cfg&gt;</Object>
  <Array key=""Key06"">
    <Element>1</Element>
    <Element>2</Element>
    <Element>3</Element>
    <Element>5</Element>
  </Array>
  <Sections key=""Key07"">
    <Section key=""Key01"">123.123321</Section>
    <Object key=""Key02"">&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
&lt;Cfg xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
  &lt;Test&gt;test string&lt;/Test&gt;
  &lt;DT&gt;2017-11-22T23:18:37.4707807+08:00&lt;/DT&gt;
&lt;/Cfg&gt;</Object>
  </Sections>
</Sections>", json.ToString());
        }

        /// <summary>
        /// Load section from xml format
        /// </summary>
        [Fact]
        public void LoadTest()
        {
            var xml = @"<Sections>
  <Section key=""Key01"">a string</Section>
  <Section key=""Key02"">1</Section>
  <Section key=""Key03"">11/22/2017 23:18:37</Section>
  <Section key=""Key04"">123.123321</Section>
  <Object key=""Key05"">&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
&lt;Cfg xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
  &lt;Test&gt;test string&lt;/Test&gt;
  &lt;DT&gt;2017-11-22T23:18:37.4591099+08:00&lt;/DT&gt;
&lt;/Cfg&gt;</Object>
  <Array key=""Key06"">
    <Element>1</Element>
    <Element>2</Element>
    <Element>3</Element>
    <Element>5</Element>
  </Array>
  <Sections key=""Key07"">
    <Section key=""Key01"">123.123321</Section>
    <Object key=""Key02"">&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
&lt;Cfg xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
  &lt;Test&gt;test string&lt;/Test&gt;
  &lt;DT&gt;2017-11-22T23:18:37.4707807+08:00&lt;/DT&gt;
&lt;/Cfg&gt;</Object>
  </Sections>
</Sections>";
            XmlConfigSection section = XmlConfigSection.Load(XElement.Parse(xml));
            Assert.Equal("a string", section.Get<string>("Key01"));
            Assert.Equal(1, section.Get<int>("Key02"));
            Assert.Equal(DateTime.Parse("11/22/2017 23:18:37"), section.Get<DateTime>("Key03"));
            Assert.Equal(123.123321m, section.Get<decimal>("Key04"));
            Assert.Equal("test string", section.Get<Cfg>("Key05").Test);
            Assert.Equal(4, section.GetList<int>("Key06").Count);
            Assert.Equal(123.123321m, section.GetSection("Key07").Get<decimal>("Key01"));
        }
    }
}
