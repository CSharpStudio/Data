%{
// ATTENTION ATTENTION ATTENTION ATTENTION
// this .CS file is a tool generated file from grammar.y and lexer.l
// DO NOT CHANGE BY HAND!!!!
// YOU HAVE BEEN WARNED !!!!

namespace QPP.Filtering.Helpers {
	using System;
	using System.Globalization;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization;
	using Css.Data.Filtering;
	using Css.Data.Filtering.Exceptions;

	/// <summary>
	///    The C# Parser
	/// </summary>
	public class CriteriaParser {
		CriteriaOperator[] result;
		public CriteriaOperator[] Result { get { return result; } }
		List<OperandValue> resultParameters = new List<OperandValue>();
		public List<OperandValue> ResultParameters { get { return resultParameters; } }
%}

	/* YACC Declarations  Cheops grammar*/
	%token CONST
	%token AGG_EXISTS AGG_COUNT AGG_MIN AGG_MAX AGG_AVG AGG_SUM
	%token PARAM
	%token COL
	%token '.'
	%token FN_ISNULL
	%token FUNCTION
	%token '[' ']'
	%token '(' ')'
	%left OR
	%left AND
	%right NOT
	%nonassoc IS NULL
	%left OP_EQ OP_NE OP_LIKE
	%left OP_GT OP_LT OP_GE OP_LE
	%nonassoc OP_IN OP_BETWEEN
	%left '|'
	%left '^'
	%left '&'
	%nonassoc '~'
	%left '-' '+'
	%left '*' '/' '%'
	%nonassoc NEG
	/* Grammar follows */
	%%
criteriaList:
	'\0'		{ result = new CriteriaOperator[0]; }
	| queryCollection '\0'	{ result = ((List<CriteriaOperator>)$1).ToArray(); }
	;

queryCollection:
	exp			{ $$ = new List<CriteriaOperator>(new CriteriaOperator[] {(CriteriaOperator)$1}); }
	| queryCollection ';' exp	{ $$ = $1; ((List<CriteriaOperator>)$$).Add((CriteriaOperator)$3); }
	;
	
upcast:
	COL			{ $$ = $1; }
	| OP_LT COL OP_GT COL			{
		OperandProperty prop2 = (OperandProperty)$2;
		OperandProperty prop4 = (OperandProperty)$4;
		prop2.PropertyName = '<' + prop2.PropertyName + '>' + prop4.PropertyName;
		$$ = prop2;
	}
	;

column:
	upcast			{ $$ = $1; }
	|	'^'		{ $$ = new OperandProperty("^"); }
	;

property:
	column		{ $$ = $1; }
	|  property '.' column	{
		OperandProperty prop1 = (OperandProperty)$1;
		OperandProperty prop3 = (OperandProperty)$3;
		prop1.PropertyName += '.' + prop3.PropertyName;
		$$ = prop1;
	}
	;

aggregate:
	property '.' aggregateSuffix	{
		AggregateOperand agg = (AggregateOperand)$3;
		$$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, null, agg.AggregateType, agg.AggregatedExpression);
	}
	|  property '[' exp ']' '.' aggregateSuffix	{
		AggregateOperand agg = (AggregateOperand)$6;
		$$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, (CriteriaOperator)$3, agg.AggregateType, agg.AggregatedExpression);
	}
	|  property '[' ']' '.' aggregateSuffix	{
		AggregateOperand agg = (AggregateOperand)$5;
		$$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, null, agg.AggregateType, agg.AggregatedExpression);
	}
	|  property '[' exp ']' { $$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, (CriteriaOperator)$3, Aggregate.Exists, null); }
	|  property '[' ']' { $$ = JoinOperand.JoinOrAggreagate((OperandProperty)$1, null, Aggregate.Exists, null); }
	|  topLevelAggregate
	;

topLevelAggregate: aggregateSuffix
	;

aggregateSuffix:
	AGG_COUNT				{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Count, null); }
	|  AGG_EXISTS			{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null); }
	|  AGG_COUNT '(' ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Count, null); }
	|  AGG_EXISTS '(' ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null); }
	|  AGG_MIN '(' exp ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Min, null); }
	|  AGG_MAX '(' exp ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Max, null); }
	|  AGG_AVG '(' exp ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Avg, null); }
	|  AGG_SUM '(' exp ')'	{ $$ = new AggregateOperand((OperandProperty)null, (CriteriaOperator)$3, Aggregate.Sum, null); }
	;

exp:
	CONST				{ $$ = $1; }
	| PARAM				{
						  string paramName = (string)$1;
						  if(string.IsNullOrEmpty(paramName)) {
						    OperandValue param = new OperandValue();
						    resultParameters.Add(param);
						    $$ = param;
						  } else {
						    bool paramNotFound = true;
						    foreach(OperandValue v in resultParameters) {
						      OperandParameter p = v as OperandParameter;
						      if(ReferenceEquals(p, null))
						        continue;
						      if(p.ParameterName != paramName)
						        continue;
						      paramNotFound = false;
						      resultParameters.Add(p);
						      $$ = p;
						      break;
						    }
						    if(paramNotFound) {
						      OperandParameter param = new OperandParameter(paramName);
						      resultParameters.Add(param);
						      $$ = param;
						    }
						  }
						}
	| property			{ $$ = $1; } 
	| aggregate			{ $$ = $1; } 
	| exp  '*'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Multiply ); }
	| exp  '/'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Divide ); }
	| exp  '+'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Plus ); }
	| exp  '-'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Minus ); }
	| exp  '%'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Modulo ); }
	| exp  '|'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.BitwiseOr ); }
	| exp  '&'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.BitwiseAnd ); }
	| exp  '^'  exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.BitwiseXor ); }
	| '-'  exp %prec NEG	{
								$$ = new UnaryOperator( UnaryOperatorType.Minus, (CriteriaOperator)$2 );
								try {
									if($2 is OperandValue) {
										OperandValue operand = (OperandValue)$2;
										if(operand.Value is Int32) {
											operand.Value = -(Int32)operand.Value;
											$$ = operand;
											break;
										} else if(operand.Value is Int64) {
											operand.Value = -(Int64)operand.Value;
											$$ = operand;
											break;
										} else if(operand.Value is Double) {
											operand.Value = -(Double)operand.Value;
											$$ = operand;
											break;
										} else if(operand.Value is Decimal) {
											operand.Value = -(Decimal)operand.Value;
											$$ = operand;
											break;
										}  else if(operand.Value is Int16) {
											operand.Value = -(Int16)operand.Value;
											$$ = operand;
											break;
										}  else if(operand.Value is SByte) {
											operand.Value = -(SByte)operand.Value;
											$$ = operand;
											break;
										}
									}
								} catch {}
							}
	| '+'  exp %prec NEG	{ $$ = new UnaryOperator( UnaryOperatorType.Plus, (CriteriaOperator)$2 ); }
	| '~'  exp				{ $$ = new UnaryOperator( UnaryOperatorType.BitwiseNot, (CriteriaOperator)$2 ); }
	| exp OP_EQ exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Equal); }
	| exp OP_NE exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.NotEqual); }
	| exp OP_GT exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Greater); }
	| exp OP_LT exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Less); }
	| exp OP_GE exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.GreaterOrEqual); }
	| exp OP_LE exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.LessOrEqual); }
	| exp OP_LIKE exp		{ $$ = new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$3, BinaryOperatorType.Like); }
	| exp NOT OP_LIKE exp %prec OP_LIKE		{ $$ = new UnaryOperator(UnaryOperatorType.Not, new BinaryOperator( (CriteriaOperator)$1, (CriteriaOperator)$4, BinaryOperatorType.Like)); }
	| NOT exp				{ $$ = new UnaryOperator(UnaryOperatorType.Not, (CriteriaOperator)$2); }
	| exp AND exp		{ $$ = GroupOperator.And((CriteriaOperator)$1, (CriteriaOperator)$3); }
	| exp OR exp		{ $$ = GroupOperator.Or((CriteriaOperator)$1, (CriteriaOperator)$3); }
	| '(' exp ')'			{ $$ = $2; }
	| exp IS NULL	{ $$ = new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)$1); }
	| exp IS NOT NULL %prec NULL	{ $$ = new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)$1)); }
	| exp OP_IN argumentslist { $$ = new InOperator((CriteriaOperator)$1, (IEnumerable<CriteriaOperator>)$3); }
	| exp OP_BETWEEN '(' exp ',' exp ')'	{ $$ = new BetweenOperator((CriteriaOperator)$1, (CriteriaOperator)$4, (CriteriaOperator)$6); }
	| FN_ISNULL '(' exp ')'	{ $$ = new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)$3); }
	| FN_ISNULL '(' exp ',' exp ')'	{ $$ = new FunctionOperator(FunctionOperatorType.IsNull, (CriteriaOperator)$3, (CriteriaOperator)$5); }	
	| FUNCTION argumentslist 	{  FunctionOperator fo = new FunctionOperator((FunctionOperatorType)$1, (IEnumerable<CriteriaOperator>)$2); lexer.CheckFunctionArgumentsCount(fo.OperatorType, fo.Operands.Count); $$ = fo; }
	| '(' ')'	{ $$ = null; }
	;	

argumentslist:
	'(' commadelimitedlist ')'	{ $$ = $2; }
	| '(' ')'					{ $$ = new List<CriteriaOperator>(); }
	;

commadelimitedlist:
	exp					{
							List<CriteriaOperator> lst = new List<CriteriaOperator>();
							lst.Add((CriteriaOperator)$1);
							$$ = lst;
						}
	| commadelimitedlist ',' exp	{
							List<CriteriaOperator> lst = (List<CriteriaOperator>)$1;
							lst.Add((CriteriaOperator)$3);
							$$ = lst;
						}
	;
%%

	CriteriaLexer lexer;

	public void yyerror (string message) {
		yyerror(message, null);
	}

	public void yyerror (string message, string[] expected) {
		string buf = message;
		if ((expected != null) && (expected.Length  > 0)) {
			buf += message;
			buf += ", expecting\n";
			for (int n = 0; n < expected.Length; ++ n)
				buf += (" "+expected[n]);
			buf += "\n";
		}
		throw new CriteriaParserException(buf);
	}

	public void Parse(String query) {
		StringReader sr = new System.IO.StringReader(query);
		lexer = new CriteriaLexer(sr);
		try {
			yyparse(lexer);
		} catch(CriteriaParserException e) {
			string malformedQuery = query;
			if(lexer.Line == 0) {
				try {
					malformedQuery = malformedQuery.Substring(0, lexer.Col) + FilteringExceptionsText.ErrorPointer + malformedQuery.Substring(lexer.Col);
				} catch { }
			}
			throw new CriteriaParserException(String.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.GrammarCatchAllErrorMessage, lexer.Line, lexer.Col, e.Message, malformedQuery), lexer.Line, lexer.Col);
		}
	}
}