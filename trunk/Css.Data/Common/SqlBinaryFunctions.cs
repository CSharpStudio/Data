using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    public class SqlBinaryFunctions
    {
        internal static bool Contains(string method)
        {
            switch (method)
            {
                case Add:
                case Subtract:
                case Multiply:
                case Divide:
                    return true;
            }
            return false;
        }
        //
        public const string Add = "+";
        public const string Subtract = "-";
        public const string Multiply = "*";
        public const string Divide = "/";
        public const string Modulo = "%";
    }
}
