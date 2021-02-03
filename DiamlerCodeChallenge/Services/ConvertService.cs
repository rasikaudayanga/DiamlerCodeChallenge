using Coding.Challenge.Firstname.Lastname;
using DiamlerCodeChallenge.Constants;
using DiamlerCodeChallenge.Models;
using HtmlAgilityPack;
using Jint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DiamlerCodeChallenge.Services
{
    public class ConvertService: IConvertService
    {
        public Engine scriptEngine { get; set; }
        public ConvertService()
        {
            scriptEngine = new Engine();
            scriptEngine.SetValue("importNamespace", new Action<string>((nameSpace) => {
                Type nsType = Type.GetType(nameSpace);
                var ins = Activator.CreateInstance(nsType);
                scriptEngine.SetValue(nsType.Name, ins);
            }));
        }

        public string GetConvertedTemplte(string htmlContent, int id)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            var htmlSource = WebUtility.HtmlDecode(htmlContent);            
            htmlDocument.LoadHtml(htmlSource);

            SetGlobleParam("request", new ScriptRequest(new { id }));

            bool isCheckScript = ProcessScriptValue(htmlDocument);

            var conditionCheckedHtmlDoc = ProcessElementWithCondition(htmlDocument);

            var repeathandledHtmlDoc = PopulateElementWithRepeaters(conditionCheckedHtmlDoc);

            var htmlString = repeathandledHtmlDoc.DocumentNode.WriteContentTo();

            string strTemp = ProcessAllElementsWithDollarSign(htmlString);
            return strTemp;
        }
              
      
        public void SetGlobleParam(string key, object value)
        {
            scriptEngine.SetValue(key, value);
        }

        public bool ProcessScriptValue(HtmlDocument htmlDoc)
        {
            var scriptNode = htmlDoc.DocumentNode.SelectNodes("//script")?.ToList()?.FirstOrDefault();
            if (scriptNode != null)
            {
                var scriptVal = scriptNode?.InnerHtml;
                scriptEngine.Execute(scriptVal);
                scriptNode.Remove();
                return true;
            }
            return true;

        }

        public HtmlDocument ProcessElementWithCondition(HtmlDocument htmlDoc)
        {
            var dataConditionElement = htmlDoc.DocumentNode.Descendants().Where(node => node.Attributes.Any(x => x.Name.Contains(Constants.Constant.DATACOND))).FirstOrDefault();
            
            var conditionVal = dataConditionElement.Attributes.First(a => a.Name == Constants.Constant.DATACOND);
            scriptEngine.Execute(conditionVal.Value);
            var dcVal = Convert.ToBoolean(scriptEngine.GetCompletionValue().ToObject());

            if (!dcVal)
                dataConditionElement.Remove();
            else
                dataConditionElement.Attributes.Remove(Constants.Constant.DATACOND);

            return htmlDoc;
        }

        public HtmlDocument PopulateElementWithRepeaters(HtmlDocument htmlDoc)
        {
            var dataRepeatElement = htmlDoc.DocumentNode.Descendants().Where(node => node.Attributes.Any(x => x.Name.Contains(Constants.Constant.DATAREPEAT))).FirstOrDefault();
            var repeatAttribute = dataRepeatElement.Attributes.FirstOrDefault(x => x.Name.Contains(Constants.Constant.DATAREPEAT));
            var attributeSuffix = repeatAttribute.Name.Replace(Constants.Constant.DATAREPEAT, string.Empty);            
            var evaluatedVal = EvaluateDollarSign(repeatAttribute.Value);
            dataRepeatElement.Attributes.Remove(repeatAttribute.Name);
            var evaluatedValAsList = ((IEnumerable)evaluatedVal).Cast<object>().ToArray();            
            foreach (var attVal in evaluatedValAsList)
            {
                var repeatableNode = dataRepeatElement.Clone();
                scriptEngine.SetValue(attributeSuffix, attVal);
                var repValName = RegexConstant.DOLLARREG.Match(dataRepeatElement.InnerText).Value;
                var finalVal = EvaluateDollarSign(repValName).ToString();
                repeatableNode.InnerHtml = repeatableNode.InnerHtml.Replace(repValName, finalVal);                
                dataRepeatElement.ParentNode.AppendChild(repeatableNode);
            }
            dataRepeatElement.Remove();
            return htmlDoc;
        }

        public string ProcessAllElementsWithDollarSign(string htmlString)
        {
            var strFinal = htmlString;
            var matchVal = RegexConstant.DOLLARREG.Matches(htmlString);
            foreach (var val in matchVal)
            {
                var strValItem = val.ToString();
                var strVal = EvaluateDollarSign(strValItem).ToString();
                strFinal = strFinal.Replace((string)strValItem, strVal);
            }
            return strFinal;
        }
        public object EvaluateDollarSign(string val)
        {
            var strVal = RegexConstant.PATTERNREG.Matches(val)[0].Value;
            scriptEngine.Execute(strVal);
            var finalVal = scriptEngine.GetCompletionValue().ToObject();
            return finalVal;
        }       
    }
}