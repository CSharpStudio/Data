using System;
using System.Runtime.Serialization;

namespace Css.Data.Filtering.Exceptions
{
	[Serializable]
	public class CriteriaParserException : Exception {
#if !CF && !SL
		protected CriteriaParserException(SerializationInfo info, StreamingContext context) : base(info, context) {
			line = info.GetInt32("line");
			column = info.GetInt32("column");
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("line", line);
			info.AddValue("column", column);
		}
#endif
		int line = -1, column = -1;
		public int Line { get { return line; } }
		public int Column { get { return column; } }
		public CriteriaParserException(string explanation) : base(explanation) { }
		public CriteriaParserException(string explanation, int line, int column) : this(explanation) {
			this.line = line;
			this.column = column;
		}
	}
	[Serializable]
	public class InvalidPropertyPathException : Exception {
#if !CF && !SL
		protected InvalidPropertyPathException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
		}
#endif
		public InvalidPropertyPathException(string messageText)
			: base(messageText) {
		}
	}
	public sealed class FilteringExceptionsText {
		public const string LexerInvalidInputCharacter = "Invalid input character \"{0}\".";
		public const string LexerNonClosedElement = "Malformed {0}: missing closing \"{1}\".";
		public const string LexerInvalidElement = "Invalid {0} value: \"{1}\".";
		public const string LexerElementPropertyName = "property name";
		public const string LexerElementStringLiteral = "string literal";
		public const string LexerElementDateTimeLiteral = "date/time literal";
		public const string LexerElementGuidLiteral = "guid literal";
		public const string LexerElementNumberLiteral = "numeric literal";
		public const string GrammarCatchAllErrorMessage = "Parser error at line {0}, character {1}: {2}; (\"{3}\")";
		public const string ErrorPointer = "{FAILED HERE}";
		public const string ExpressionEvaluatorOperatorSubtypeNotImplemented = "ICriteriaProcessor.ProcessOperator({0} '{1}') not implemented";
		public const string ExpressionEvaluatorAnalyzePatternInvalidPattern = "Invalid argument '{0}'";
		public const string ExpressionEvaluatorInvalidPropertyPath = "Can't find property '{0}'";
		public const string ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType = "{0} {1} not supported for type {2}";
		public const string ExpressionEvaluatorNotACollectionPath = "'{0}' doesn't implement ITypedList";
		public const string ExpressionEvaluatorJoinOperandNotSupported = "JoinOperand not supported";
	}
}
