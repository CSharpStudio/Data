using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    /// <summary>
    /// SQL函数
    /// </summary>
    public class SqlFunctions
    {
        public static bool Contains(string method)
        {
            switch (method)
            {
                case COUNT:
                case NVL:
                case MAX:
                case MIN:
                case AVG:
                case SUM:
                case UPPER:
                case LOWER:
                case SUBSTR:
                case LTRIM:
                case RTRIM:
                case LENGTH:
                case CONCAT:
                case DAY:
                case YEAR:
                case MONTH:
                case ABS:
                case SIN:
                case COS:
                case TAN:
                case ASIN:
                case ACOS:
                case ATAN:
                case SIGN:
                case POWER:
                case SQRT:
                case EXP:
                case ROUND:
                case FLOOR:
                case CEILING:
                    return true;
            }
            return false;
        }
        public const string NVL = "NVL";
        public const string COUNT = "COUNT";
        //Group
        public const string MAX = "MAX";
        public const string MIN = "MIN";
        public const string AVG = "AVG";
        public const string SUM = "SUM";
        //String
        public const string UPPER = "UPPER";
        public const string LOWER = "LOWER";
        public const string SUBSTR = "SUBSTR";
        public const string LTRIM = "LTRIM";
        public const string RTRIM = "RTRIM";
        public const string LENGTH = "LENGTH";
        public const string CONCAT = "CONCAT";
        //DateTime
        public const string DAY = "DAY";
        public const string YEAR = "YEAR";
        public const string MONTH = "MONTH";
        //Math
        public const string ABS = "ABS";
        public const string SIN = "SIN";
        public const string COS = "COS";
        public const string TAN = "TAN";
        public const string ASIN = "ASIN";
        public const string ACOS = "ACOS";
        public const string ATAN = "ATAN";
        public const string SIGN = "SIGN";
        public const string POWER = "POWER";
        public const string SQRT = "SQRT";
        public const string EXP = "EXP";
        public const string ROUND = "ROUND";
        public const string FLOOR = "FLOOR";
        public const string CEILING = "CEILING";

    }
}
