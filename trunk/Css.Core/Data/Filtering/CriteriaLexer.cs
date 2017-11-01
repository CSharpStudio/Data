using System;
using System.Globalization;
using System.IO;
using Css.Data.Filtering;
using Css.Data.Filtering.Exceptions;

namespace Css.Data.Filtering.Helpers
{
	public class CriteriaLexer : yyInput {
		protected readonly TextReader InputReader;
		public int CurrentToken = 0;
		public object CurrentValue = null;
		bool isAfterColumn = false;
		int _line = -1;
		int _col = -1;
		int _currentLine = 0;
		int _currentCol = 0;
		int _pos = 0;
		public int Line { get { return _line; } }
		public int Col { get { return _col; } }
		public int Position { get { return _pos; } }
		bool yyInput.advance() {
			return this.Advance();
		}
		int yyInput.token() {
			return CurrentToken;
		}
		object yyInput.value() {
			return CurrentValue;
		}
		public CriteriaLexer(TextReader inputReader) {
			this.InputReader = inputReader;
		}
		public bool Advance() {
			SkipBlanks();
			_line = _currentLine;
			_col = _currentCol;
			CurrentToken = 0;
			CurrentValue = null;
			int nextInt = ReadNextChar();
			if(nextInt == -1) {
				return false;
			}
			char nextChar = (char)nextInt;
			switch(nextChar) {
				case '?':
					DoParam();
					break;
				case '^':
				case '+':
				case '*':
				case '/':
				case '%':
				case '(':
				case ')':
				case ']':
				case ',':
				case '~':
				case '-':
				case ';':
					this.CurrentToken = nextChar;
					break;
				case '.':
					DoDotOrNumber();
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					DoNumber(nextChar);
					break;
				case '!':
					if(PeekNextChar() == '=') {
						ReadNextChar();
						this.CurrentToken = Token.OP_NE;
					} else {
						this.CurrentToken = Token.NOT;
					}
					break;
				case '=':
					this.CurrentToken = Token.OP_EQ;
					if(PeekNextChar() == '=') {
						ReadNextChar();
					}
					break;
				case '<':
					if(PeekNextChar() == '>') {
						ReadNextChar();
						this.CurrentToken = Token.OP_NE;
					} else if(PeekNextChar() == '=') {
						ReadNextChar();
						this.CurrentToken = Token.OP_LE;
					} else {
						this.CurrentToken = Token.OP_LT;
					}
					break;
				case '>':
					if(PeekNextChar() == '=') {
						ReadNextChar();
						this.CurrentToken = Token.OP_GE;
					} else {
						this.CurrentToken = Token.OP_GT;
					}
					break;
				case '|':
					if(PeekNextChar() == '|') {
						ReadNextChar();
						this.CurrentToken = Token.OR;
					} else {
						this.CurrentToken = nextChar;
					}
					break;
				case '&':
					if(PeekNextChar() == '&') {
						ReadNextChar();
						this.CurrentToken = Token.AND;
					} else {
						this.CurrentToken = nextChar;
					}
					break;
				case '[':
					if(isAfterColumn) {
						this.CurrentToken = nextChar;
					} else {
						DoEnclosedColumn();
					}
					break;
				case '{':
					DoConstGuid();
					break;
				case '\'':
					DoString();
					break;
				case '@':
					DoAtColumn();
					break;
				case '#':
					DoDateTimeConst();
					break;
				default:
					CatchAll(nextChar);
					break;
			}
			isAfterColumn = this.CurrentToken == Token.COL;
			return true;
		}
		int wasChar = 0;
		protected int ReadNextChar() {
			int nextInt = InputReader.Read();
			if(nextInt == -1) {
				_pos--;
				wasChar = 0;
			} else if(nextInt == '\n') {
				if(wasChar == '\r') {
					_pos--;
					wasChar = 0;
				} else {
					wasChar = nextInt;
					++_currentLine;
					_currentCol = 0;
				}
			} else if(nextInt == '\r') {
				if(wasChar == '\n') {
					_pos--;
					wasChar = 0;
				} else {
					wasChar = nextInt;
					++_currentLine;
					_currentCol = 0;
				}
			} else {
				wasChar = 0;
				++_currentCol;
			}
			_pos++;
			return nextInt;
		}
		protected int PeekNextChar() {
			return InputReader.Peek();
		}
		public void SkipBlanks() {
			for(; ; ) {
				switch(PeekNextChar()) {
					case '\n':
					case '\r':
					case '\t':
					case '\b':
					case ' ':
						ReadNextChar();
						break;
					default:
						return;
				}
			}
		}
		void DoAtColumn() {
			string columnName = string.Empty;
			for(; ; ) {
				if(CanContinueColumn((char)PeekNextChar())) {
					columnName += (char)ReadNextChar();
				} else
					break;
			}
			this.CurrentToken = Token.COL;
			this.CurrentValue = new OperandProperty(columnName);
		}
		void DoParam() {
			string paramName = null;
			for(; ; ) {
				if(CanContinueColumn((char)PeekNextChar())) {
					paramName += (char)ReadNextChar();
				} else
					break;
			}
			this.CurrentToken = Token.PARAM;
			this.CurrentValue = paramName;
		}
		void DoEnclosedColumn() {
			string name = string.Empty;
			this.CurrentToken = Token.COL;
			try {
				for(; ; ) {
					int nextInt = ReadNextChar();
					if(nextInt == -1) {
						YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementPropertyName, "]");
						return;
					}
					char nextChar = (char)nextInt;
					if(nextChar == ']') {
						return;
					}
					if(nextChar == '\\') {
						nextInt = ReadNextChar();
						if(nextInt == -1) {
							YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementPropertyName, "]");
							return;
						}
						nextChar = (char)nextInt;
						switch(nextChar) {
							case 'n':
								name += '\n';
								break;
							case 'r':
								name += '\r';
								break;
							case 't':
								name += '\t';
								break;
							default:
								name += nextChar;
								break;
						}
					} else {
						name += nextChar;
					}
				}
			} finally {
				this.CurrentValue = new OperandProperty(name);
			}
		}
		void DoString() {
			this.CurrentToken = Token.CONST;
			string str = string.Empty;
			for(; ; ) {
				int nextInt = ReadNextChar();
				if(nextInt == -1) {
					this.CurrentValue = new OperandValue(str);
					YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementStringLiteral, "'");
					return;
				}
				char nextChar = (char)nextInt;
				if(nextChar == '\'') {
					if(PeekNextChar() != '\'') {
						this.CurrentValue = new OperandValue(str);
						if(str.Length == 1) {
							int possibleSuffix = PeekNextChar();
							if(possibleSuffix == 'c' || possibleSuffix == 'C') {
								ReadNextChar();
								this.CurrentValue = new OperandValue(str[0]);
							}
						}
						return;
					}
					ReadNextChar();
				}
				str += nextChar;
			}
		}
		void DoDateTimeConst() {
			this.CurrentToken = Token.CONST;
			string str = string.Empty;
			for(; ; ) {
				int nextInt = ReadNextChar();
				if(nextInt == -1) {
					this.CurrentValue = new OperandValue(str);
					YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementDateTimeLiteral, "#");
					return;
				}
				char nextChar = (char)nextInt;
				if(nextChar == '#')
					break;
				str += nextChar;
			}
#if DXWhidbey && !CF
			TimeSpan ts;
			if(TimeSpan.TryParse(str, out ts)) {
				this.CurrentValue = new OperandValue(ts);
				return;
			}
			DateTime dt;
			if(DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dt)) {
				this.CurrentValue = new OperandValue(dt);
				return;
			}
#else
			try {
				this.CurrentValue = new OperandValue(TimeSpan.Parse(str));
				return;
			} catch { }
			try {
				this.CurrentValue = new OperandValue(DateTime.Parse(str, CultureInfo.InvariantCulture));
				return;
			} catch { }
#endif
			this.CurrentValue = new OperandValue(str);
			YYError(FilteringExceptionsText.LexerInvalidElement, FilteringExceptionsText.LexerElementDateTimeLiteral, str);
		}
		void DoConstGuid() {
			this.CurrentToken = Token.CONST;
			string str = string.Empty;
			for(; ; ) {
				int nextInt = ReadNextChar();
				if(nextInt == -1) {
					this.CurrentValue = new OperandValue(str);
					YYError(FilteringExceptionsText.LexerNonClosedElement, FilteringExceptionsText.LexerElementGuidLiteral, "}");
					return;
				}
				char nextChar = (char)nextInt;
				if(nextChar == '}')
					break;
				str += nextChar;
			}
			try {
				this.CurrentValue = new OperandValue(new Guid(str));
				return;
			} catch { }
			this.CurrentValue = new OperandValue(str);
			YYError(FilteringExceptionsText.LexerInvalidElement, FilteringExceptionsText.LexerElementGuidLiteral, str);
		}
		void CatchAll(char firstChar) {
			string str = string.Empty;
			str += firstChar;
			if(!CanStartColumn(firstChar)) {
				this.CurrentToken = Token.yyErrorCode;
				this.CurrentValue = firstChar;
				YYError(FilteringExceptionsText.LexerInvalidInputCharacter, str);
				return;
			}
			for(; ; ) {
				int nextInt = PeekNextChar();
				if(nextInt == -1)
					break;
				char nextChar = (char)nextInt;
				if(!CanContinueColumn(nextChar))
					break;
				ReadNextChar();
				str += nextChar;
			}
			switch(str.ToUpper(CultureInfo.InvariantCulture)) {
				case "AND":
					this.CurrentToken = Token.AND;
					break;
				case "OR":
					this.CurrentToken = Token.OR;
					break;
				case "TRUE":
					this.CurrentToken = Token.CONST;
					this.CurrentValue = new OperandValue(true);
					break;
				case "FALSE":
					this.CurrentToken = Token.CONST;
					this.CurrentValue = new OperandValue(false);
					break;
				case "NOT":
					this.CurrentToken = Token.NOT;
					break;
				case "IS":
					this.CurrentToken = Token.IS;
					break;
				case "NULL":
					this.CurrentToken = Token.NULL;
					break;
				case "LIKE":
					this.CurrentToken = Token.OP_LIKE;
					break;
				case "ISNULL":
					this.CurrentToken = Token.FN_ISNULL;
					break;
				case "ISNULLOREMPTY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsNullOrEmpty;
					break;
				case "TRIM":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Trim;
					break;
				case "LEN":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Len;
					break;
				case "SUBSTRING":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Substring;
					break;
				case "UPPER":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Upper;
					break;
				case "LOWER":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Lower;
					break;
				case "CUSTOM":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Custom;
					break;
				case "CONCAT":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Concat;
					break;
				case "IIF":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Iif;
					break;
				case "ABS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Abs;
					break;
				case "ACOS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Acos;
					break;
				case "ADDDAYS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddDays;
					break;
				case "ADDHOURS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddHours;
					break;
				case "ADDMILLISECONDS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddMilliSeconds;
					break;
				case "ADDMINUTES":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddMinutes;
					break;
				case "ADDMONTHS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddMonths;
					break;
				case "ADDSECONDS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddSeconds;
					break;
				case "ADDTICKS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddTicks;
					break;
				case "ADDTIMESPAN":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddTimeSpan;
					break;
				case "ADDYEARS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.AddYears;
					break;
				case "ASCII":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Ascii;
					break;
				case "ASIN":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Asin;
					break;
				case "ATN":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Atn;
					break;
				case "ATN2":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Atn2;
					break;
				case "BIGMUL":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.BigMul;
					break;
				case "CEILING":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Ceiling;
					break;
				case "CHAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Char;
					break;
				case "CHARINDEX":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.CharIndex;
					break;
				case "COS":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Cos;
					break;
				case "COSH":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Cosh;
					break;
				case "EXP":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Exp;
					break;
				case "FLOOR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Floor;
					break;
				case "GETDATE":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetDate;
					break;
				case "GETDAY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetDay;
					break;
				case "GETDAYOFWEEK":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetDayOfWeek;
					break;
				case "GETDAYOFYEAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetDayOfYear;
					break;
				case "GETHOUR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetHour;
					break;
				case "GETMILLISECOND":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetMilliSecond;
					break;
				case "GETMINUTE":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetMinute;
					break;
				case "GETMONTH":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetMonth;
					break;
				case "GETSECOND":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetSecond;
					break;
				case "GETTIMEOFDAY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetTimeOfDay;
					break;
				case "GETYEAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.GetYear;
					break;
				case "LOG":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Log;
					break;
				case "LOG10":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Log10;
					break;
				case "NOW":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Now;
					break;
				case "UTCNOW":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.UtcNow;
					break;
				case "PADLEFT":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.PadLeft;
					break;
				case "PADRIGHT":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.PadRight;
					break;
				case "POWER":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Power;
					break;
				case "REMOVE":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Remove;
					break;
				case "REPLACE":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Replace;
					break;
				case "REVERSE":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Reverse;
					break;
				case "RND":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Rnd;
					break;
				case "ROUND":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Round;
					break;
				case "SIGN":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Sign;
					break;
				case "SIN":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Sin;
					break;
				case "SINH":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Sinh;
					break;
				case "SQR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Sqr;
					break;
				case "TOSTR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.ToStr;
					break;
				case "INSERT":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Insert;
					break;
				case "TAN":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Tan;
					break;
				case "TANH":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Tanh;
					break;
				case "TODAY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.Today;
					break;
				case "BETWEEN":
					this.CurrentToken = Token.OP_BETWEEN;
					break;
				case "IN":
					this.CurrentToken = Token.OP_IN;
					break;
				case "EXISTS":
					this.CurrentToken = Token.AGG_EXISTS;
					break;
				case "COUNT":
					this.CurrentToken = Token.AGG_COUNT;
					break;
				case "MIN":
					this.CurrentToken = Token.AGG_MIN;
					break;
				case "MAX":
					this.CurrentToken = Token.AGG_MAX;
					break;
				case "AVG":
					this.CurrentToken = Token.AGG_AVG;
					break;
				case "SUM":
					this.CurrentToken = Token.AGG_SUM;
					break;
				case "LOCALDATETIMETHISYEAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeThisYear;
					break;
				case "LOCALDATETIMETHISMONTH":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeThisMonth;
					break;
				case "LOCALDATETIMELASTWEEK":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeLastWeek;
					break;
				case "LOCALDATETIMETHISWEEK":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeThisWeek;
					break;
				case "LOCALDATETIMEYESTERDAY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeYesterday;
					break;
				case "LOCALDATETIMETODAY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeToday;
					break;
				case "LOCALDATETIMENOW":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeNow;
					break;
				case "LOCALDATETIMETOMORROW":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeTomorrow;
					break;
				case "LOCALDATETIMEDAYAFTERTOMORROW":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeDayAfterTomorrow;
					break;
				case "LOCALDATETIMENEXTWEEK":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeNextWeek;
					break;
				case "LOCALDATETIMETWOWEEKSAWAY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeTwoWeeksAway;
					break;
				case "LOCALDATETIMENEXTMONTH":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeNextMonth;
					break;
				case "LOCALDATETIMENEXTYEAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.LocalDateTimeNextYear;
					break;
				case "ISOUTLOOKINTERVALBEYONDTHISYEAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalBeyondThisYear;
					break;
				case "ISOUTLOOKINTERVALLATERTHISYEAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalLaterThisYear;
					break;
				case "ISOUTLOOKINTERVALLATERTHISMONTH":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalLaterThisMonth;
					break;
				case "ISOUTLOOKINTERVALNEXTWEEK":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalNextWeek;
					break;
				case "ISOUTLOOKINTERVALLATERTHISWEEK":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalLaterThisWeek;
					break;
				case "ISOUTLOOKINTERVALTOMORROW":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalTomorrow;
					break;
				case "ISOUTLOOKINTERVALTODAY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalToday;
					break;
				case "ISOUTLOOKINTERVALYESTERDAY":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalYesterday;
					break;
				case "ISOUTLOOKINTERVALEARLIERTHISWEEK":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalEarlierThisWeek;
					break;
				case "ISOUTLOOKINTERVALLASTWEEK":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalLastWeek;
					break;
				case "ISOUTLOOKINTERVALEARLIERTHISMONTH":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalEarlierThisMonth;
					break;
				case "ISOUTLOOKINTERVALEARLIERTHISYEAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalEarlierThisYear;
					break;
				case "ISOUTLOOKINTERVALPRIORTHISYEAR":
					this.CurrentToken = Token.FUNCTION;
					this.CurrentValue = FunctionOperatorType.IsOutlookIntervalPriorThisYear;
					break;
				default:
					this.CurrentToken = Token.COL;
					this.CurrentValue = new OperandProperty(str);
					break;
			}
		}
		void DoNumber(char firstSymbol) {
			string str = string.Empty;
			str += firstSymbol;
			for(; ; ) {
				int nextInt = PeekNextChar();
				char nextChar = (char)nextInt;
				switch(nextChar) {
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case '.':
						ReadNextChar();
						str += nextChar;
						break;
					case 'e':
					case 'E':
						ReadNextChar();
						str += nextChar;
						nextInt = ReadNextChar();
						if(nextInt == -1) {
							YYError(FilteringExceptionsText.LexerInvalidElement, FilteringExceptionsText.LexerElementNumberLiteral, str);
							break;
						}
						nextChar = (char)nextInt;
						str += nextChar;
						break;
					default:
						this.CurrentToken = Token.CONST;
						string numericCode = GetNumericCode();
						try {
							this.CurrentValue = new OperandValue(ExtractNumericValue(str, numericCode));
						} catch {
							this.CurrentValue = new OperandValue(str + numericCode);
							YYError(FilteringExceptionsText.LexerInvalidElement, FilteringExceptionsText.LexerElementNumberLiteral, str + numericCode);
						}
						return;
				}
			}
		}
		object ExtractNumericValue(string str, string numericCode) {
			switch(numericCode.ToLower(CultureInfo.InvariantCulture)) {
				case "m":
					return Convert.ToDecimal(str, CultureInfo.InvariantCulture);
				case "f":
					return Convert.ToSingle(str, CultureInfo.InvariantCulture);
				case "i":
					return Convert.ToInt32(str, CultureInfo.InvariantCulture);
				case "s":
					return Convert.ToInt16(str, CultureInfo.InvariantCulture);
				case "l":
					return Convert.ToInt64(str, CultureInfo.InvariantCulture);
				case "b":
					return Convert.ToByte(str, CultureInfo.InvariantCulture);
				case "u":
				case "ui":
				case "iu":
					return Convert.ToUInt32(str, CultureInfo.InvariantCulture);
				case "sb":
				case "bs":
					return Convert.ToSByte(str, CultureInfo.InvariantCulture);
				case "us":
				case "su":
					return Convert.ToUInt16(str, CultureInfo.InvariantCulture);
				case "ul":
				case "lu":
					return Convert.ToUInt64(str, CultureInfo.InvariantCulture);
				default:
					throw new InvalidOperationException("invalid type code");
				case "":
					if(str.IndexOfAny(new char[] { '.', 'e', 'E' }) >= 0)
						return Convert.ToDouble(str, CultureInfo.InvariantCulture);
					try {
						return Convert.ToInt32(str, CultureInfo.InvariantCulture);
					} catch { }
					try {
						return Convert.ToInt64(str, CultureInfo.InvariantCulture);
					} catch { }
					return Convert.ToDouble(str, CultureInfo.InvariantCulture);
			}
		}
		string GetNumericCode() {
			int peeked = PeekNextChar();
			if(peeked == -1)
				return string.Empty;
			char ch = (char)peeked;
			switch(ch) {
				default:
					return string.Empty;
				case 'm':
				case 'M':
				case 'f':
				case 'F':
					ReadNextChar();
					return ch.ToString();
				case 'b':
				case 's':
				case 'i':
				case 'l':
				case 'u':
				case 'B':
				case 'S':
				case 'I':
				case 'L':
				case 'U':
					break;
			}
			ReadNextChar();
			peeked = PeekNextChar();
			if(peeked != -1) {
				char ch2 = (char)peeked;
				switch(ch2) {
					case 'b':
					case 's':
					case 'i':
					case 'l':
					case 'u':
					case 'B':
					case 'S':
					case 'I':
					case 'L':
					case 'U':
						ReadNextChar();
						return ch.ToString() + ch2.ToString();
				}
			}
			return ch.ToString();
		}
		void DoDotOrNumber() {
			switch(PeekNextChar()) {
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					DoNumber('.');
					break;
				default:
					this.CurrentToken = '.';
					break;
			}
		}
		public static bool CanStartColumn(char value) {
			switch(char.GetUnicodeCategory(value)) {
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.ConnectorPunctuation:
					return true;
				default:
					return false;
			}
		}
		public static bool CanContinueColumn(char value) {
			switch(char.GetUnicodeCategory(value)) {
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.ConnectorPunctuation:
				case UnicodeCategory.DecimalDigitNumber:
				case UnicodeCategory.LetterNumber:
				case UnicodeCategory.OtherNumber:
					return true;
				default:
					return false;
			}
		}
		public virtual void YYError(string message, params object[] args) {
			string fullMessage = string.Format(CultureInfo.InvariantCulture, message, args);
			throw new CriteriaParserException(fullMessage);
		}
		public void CheckFunctionArgumentsCount(FunctionOperatorType functionType, int argumentsCount){
			if ((functionType == FunctionOperatorType.Custom || functionType == FunctionOperatorType.Concat) && argumentsCount > 0) return;
			switch (argumentsCount) {
				case 0:
					switch (functionType) {
						case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
						case FunctionOperatorType.LocalDateTimeLastWeek:
						case FunctionOperatorType.LocalDateTimeNextMonth:
						case FunctionOperatorType.LocalDateTimeNextWeek:
						case FunctionOperatorType.LocalDateTimeNextYear:
						case FunctionOperatorType.LocalDateTimeNow:
						case FunctionOperatorType.LocalDateTimeThisMonth:
						case FunctionOperatorType.LocalDateTimeThisWeek:
						case FunctionOperatorType.LocalDateTimeThisYear:
						case FunctionOperatorType.LocalDateTimeToday:
						case FunctionOperatorType.LocalDateTimeTomorrow:
						case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
						case FunctionOperatorType.LocalDateTimeYesterday:
						case FunctionOperatorType.Now:
						case FunctionOperatorType.UtcNow:
						case FunctionOperatorType.Today:
						case FunctionOperatorType.Rnd:
							return;
					}
					break;
				case 1:
					switch (functionType) {
						case FunctionOperatorType.IsNullOrEmpty:
						case FunctionOperatorType.Trim:
						case FunctionOperatorType.Upper:
						case FunctionOperatorType.Lower:
						case FunctionOperatorType.Len:
						case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
						case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
						case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
						case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
						case FunctionOperatorType.IsOutlookIntervalLastWeek:
						case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
						case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
						case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
						case FunctionOperatorType.IsOutlookIntervalNextWeek:
						case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
						case FunctionOperatorType.IsOutlookIntervalToday:
						case FunctionOperatorType.IsOutlookIntervalTomorrow:
						case FunctionOperatorType.IsOutlookIntervalYesterday:
						case FunctionOperatorType.Ascii:
						case FunctionOperatorType.Char:
						case FunctionOperatorType.ToStr:
						case FunctionOperatorType.Reverse:
						case FunctionOperatorType.Abs:
						case FunctionOperatorType.Sqr:
						case FunctionOperatorType.Cos:
						case FunctionOperatorType.Sin:
						case FunctionOperatorType.Atn:
						case FunctionOperatorType.Exp:
						case FunctionOperatorType.Log:
						case FunctionOperatorType.Log10:
						case FunctionOperatorType.Tan:
						case FunctionOperatorType.Sign:
						case FunctionOperatorType.Round:
						case FunctionOperatorType.Ceiling:
						case FunctionOperatorType.Floor:
						case FunctionOperatorType.Asin:
						case FunctionOperatorType.Acos:
						case FunctionOperatorType.Cosh:
						case FunctionOperatorType.Sinh:
						case FunctionOperatorType.Tanh:
						case FunctionOperatorType.GetDate:
						case FunctionOperatorType.GetDay:
						case FunctionOperatorType.GetDayOfWeek:
						case FunctionOperatorType.GetDayOfYear:
						case FunctionOperatorType.GetHour:
						case FunctionOperatorType.GetMilliSecond:
						case FunctionOperatorType.GetMinute:
						case FunctionOperatorType.GetMonth:
						case FunctionOperatorType.GetSecond:
						case FunctionOperatorType.GetTimeOfDay:
						case FunctionOperatorType.GetYear:
							return;
					}
					break;
				case 2:
					switch (functionType) {
						case FunctionOperatorType.Substring:
						case FunctionOperatorType.Log:
						case FunctionOperatorType.Power:
						case FunctionOperatorType.Atn2:
						case FunctionOperatorType.BigMul:
						case FunctionOperatorType.PadLeft:
						case FunctionOperatorType.PadRight:
						case FunctionOperatorType.AddDays:
						case FunctionOperatorType.AddHours:
						case FunctionOperatorType.AddMilliSeconds:
						case FunctionOperatorType.AddMinutes:
						case FunctionOperatorType.AddMonths:
						case FunctionOperatorType.AddSeconds:
						case FunctionOperatorType.AddTicks:
						case FunctionOperatorType.AddTimeSpan:
						case FunctionOperatorType.AddYears:
						case FunctionOperatorType.CharIndex:
							return;
					}
					break;
				case 3:
					switch (functionType) {
						case FunctionOperatorType.Substring:
						case FunctionOperatorType.Iif:
						case FunctionOperatorType.PadLeft:
						case FunctionOperatorType.PadRight:
						case FunctionOperatorType.CharIndex:
						case FunctionOperatorType.Insert:
						case FunctionOperatorType.Remove:
						case FunctionOperatorType.Replace:
							return;
					}
					break;
				case 4:
					if (functionType == FunctionOperatorType.CharIndex) return;
					break;
			}
			YYError("Wrong arguments count ({0}). Function - '{1}'.", argumentsCount, functionType.ToString());
		}
	}
}
