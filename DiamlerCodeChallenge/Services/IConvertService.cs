using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamlerCodeChallenge.Services
{
    public interface IConvertService
    {
        string GetConvertedTemplte(string htmlContent, int id);

        HtmlDocument PopulateElementWithRepeaters(HtmlDocument html);
        bool ProcessScriptValue(HtmlDocument html);
        HtmlDocument ProcessElementWithCondition(HtmlDocument html);
    }
}
