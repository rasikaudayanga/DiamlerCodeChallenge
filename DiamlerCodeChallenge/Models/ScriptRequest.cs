using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiamlerCodeChallenge.Models
{
    public class ScriptRequest
    {
        dynamic requestParams;

        public ScriptRequest(dynamic parameters)
        {
            requestParams = parameters;
        }

        public object getParameter(string parameter)
        {
            var val = requestParams.GetType().GetProperty(parameter).GetValue(requestParams, null);
            return val;
        }
    }
}