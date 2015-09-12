using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Wox.Plugin.Boromak
{
    class QueryHandler
    {
        static string _regexPattern = "(([A-z])+)|(\".+\")";
        private List<string> arguments; 
        public QueryHandler(Query query)
        {
           DecomposeQuery(query.RawQuery);
        }

        public static implicit operator List<string>(QueryHandler q)
        {
            return q.arguments;
        }

       

        private void DecomposeQuery(string query)
        {
            Regex regex = new Regex(_regexPattern);
            var matches = regex.Matches(query);
            foreach (Match m in matches)
            {
                arguments.Add(m.Value);
            }
        }

        public string this[int index]
        {
            get { return arguments[index]; }
            
        }
        public int Count
        {
            get { return arguments.Count; }
        }
        public string GetArgumentAt(int index)
        {
            if (index >= arguments.Count) return String.Empty;
            return arguments.ElementAt(index);
        }

        public string GetAllAfter(int index)
        {
            if (index >= arguments.Count) return String.Empty;
            return String.Join(" ", arguments.Skip(index).ToArray());
        }
    }
}
