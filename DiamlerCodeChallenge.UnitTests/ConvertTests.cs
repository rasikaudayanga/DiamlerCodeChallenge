using DiamlerCodeChallenge.Services;
using HtmlAgilityPack;
using Jint;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Tests
{
    public class ConvertTests
    {
        private ConvertService _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new ConvertService();
        }

        #region TestScriptContent
        [Test]
        public void TestScriptContentWithScriptElement()
        {
            var file = WebUtility.HtmlDecode("<!DOCTYPE html><html><script type=\"server/javascript\"></script></html>");
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(file);
            var result = _converter.ProcessScriptValue(htmlDocument);
            Assert.IsTrue(result);
        }
        #endregion

        #region TestOtherAttribute

        [Test]
        public void TestConditionalElementsWithCondAttribute()
        {
            var file = WebUtility.HtmlDecode("<div data-cond=\"isCheck\">Test Data Condition</div>");
            HtmlDocument htmlDocument = new HtmlDocument();
            _converter.scriptEngine.SetValue("isCheck", true);           
            htmlDocument.LoadHtml(file);
            var output = _converter.ProcessElementWithCondition(htmlDocument);

            Assert.IsTrue(output.DocumentNode.Descendants().Where(node => node.Name.Equals("div") && node.InnerText.Equals("Test Data Condition")).Count() > 0);
            Assert.IsTrue(output.DocumentNode.Descendants().Where(node => node.Attributes.Any(x => x.Name.Equals("data-cond"))).Count() == 0);
        }

        [Test]
        public void TestConditionalElementsWithoutCondAttribute()
        {
            _converter.scriptEngine.SetValue("isCheck", true);
            var file = WebUtility.HtmlDecode("<div>Test Data Condition</div>");
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(file);
            var output = _converter.ProcessElementWithCondition(htmlDocument);

            Assert.IsTrue(output.DocumentNode.Descendants().Where(node => node.Name.Equals("div") && node.InnerText.Equals("Test Data Condition")).Count() > 0);
            Assert.IsTrue(output.DocumentNode.Descendants().Where(node => node.Attributes.Any(x => !x.Name.Equals("data-cond"))).Count() == 0);
        }

        #endregion

        #region TestRepeatableAttribute
        [Test]
        public void TestRepeatableElementWithRepeatAttribute()
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            var file = WebUtility.HtmlDecode("<!DOCTYPE html><html><script type=\"server/javascript\"></script><div data-repeat-model=\"vehicles\">${model}</div></html>");
            var vehicleList = new List<string> { "car", "bus" };
            _converter.scriptEngine.SetValue("vehicles", vehicleList);

            htmlDocument.LoadHtml(file);
            var returnedNodes = _converter.PopulateElementWithRepeaters(htmlDocument);
           
            Assert.IsFalse(returnedNodes.DocumentNode.Descendants().Any(node => node.Attributes.Any(x => x.Name.Contains("data-repeat-model"))));
            Assert.IsTrue(returnedNodes.DocumentNode.ChildNodes.Count == 2);
        }        
        #endregion        
    }
}