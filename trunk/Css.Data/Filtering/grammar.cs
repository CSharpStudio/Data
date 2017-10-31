#line 2 "grammar.y"
namespace Css.Data.Filtering.Helpers
{
	using System;
	using System.Globalization;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization;
    using Css.Data.Filtering;
    using Css.Data.Filtering.Exceptions;
	public class CriteriaParser {
		CriteriaOperator[] result;
		public CriteriaOperator[] Result { get { return result; } }
		List<OperandValue> resultParameters = new List<OperandValue>();
		public List<OperandValue> ResultParameters { get { return resultParameters; } }
#line default
  int yyMax;
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CS0162")]
  Object yyparse (yyInput yyLex) {
	if (yyMax <= 0) yyMax = 256;			
	int yyState = 0;								   
	int [] yyStates = new int[yyMax];					
	Object yyVal = null;							   
	Object [] yyVals = new Object[yyMax];			
	int yyToken = -1;					
	int yyErrorFlag = 0;				
	int yyTop = 0;
	goto skip;
	yyLoop:
	yyTop++;
	skip:
	for(;; ++yyTop) {
	  if(yyTop >= yyStates.Length) {			
		int[] i = new int[yyStates.Length + yyMax];
		yyStates.CopyTo(i, 0);
		yyStates = i;
		Object[] o = new Object[yyVals.Length + yyMax];
		yyVals.CopyTo(o, 0);
		yyVals = o;
	  }
	  yyStates[yyTop] = yyState;
	  yyVals[yyTop] = yyVal;
	  yyDiscarded:	
	  for(;;) {
		int yyN;
		if ((yyN = yyDefRed[yyState]) == 0) {	
		  if(yyToken < 0)
			yyToken = yyLex.advance() ? yyLex.token() : 0;
		  if((yyN = yySindex[yyState]) != 0 && ((yyN += yyToken) >= 0)
			  && (yyN < yyTable.Length) && (yyCheck[yyN] == yyToken)) {
			yyState = yyTable[yyN];		
			yyVal = yyLex.value();
			yyToken = -1;
			if (yyErrorFlag > 0) -- yyErrorFlag;
			goto yyLoop;
		  }
		  if((yyN = yyRindex[yyState]) != 0 && (yyN += yyToken) >= 0
			  && yyN < yyTable.Length && yyCheck[yyN] == yyToken)
			yyN = yyTable[yyN];			
		  else
			switch(yyErrorFlag) {
			case 0:
			  yyerror("syntax error");
			  goto case 1;
			case 1: case 2:
			  yyErrorFlag = 3;
			  do {
				if((yyN = yySindex[yyStates[yyTop]]) != 0
					&& (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
					&& yyCheck[yyN] == Token.yyErrorCode) {
				  yyState = yyTable[yyN];
				  yyVal = yyLex.value();
				  goto yyLoop;
				}
			  } while (--yyTop >= 0);
			  yyerror("irrecoverable syntax error");
			  goto yyDiscarded;
			case 3:
			  if (yyToken == 0)
				yyerror("irrecoverable syntax error at end-of-file");
			  yyToken = -1;
			  goto yyDiscarded;		
			}
		}
		int yyV = yyTop + 1 - yyLen[yyN];
		yyVal = yyV > yyTop ? null : yyVals[yyV];
		switch(yyN) {
case 1:
#line 54 "grammar.y"
  { result = new CriteriaOperator[0]; }
  break;
case 2:
#line 55 "grammar.y"
  { result = ((List<CriteriaOperator>)yyVals[-1+yyTop]).ToArray(); }
  break;
case 3:
#line 59 "grammar.y"
  { yyVal = new List<CriteriaOperator>(new CriteriaOperator[] {(CriteriaOperator)yyVals[0+yyTop]}); }
  break;
case 4:
#line 60 "grammar.y"
  { yyVal = yyVals[-2+yyTop]; ((List<CriteriaOperator>)yyVal).Add((CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 5:
#line 64 "grammar.y"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 6:
#line 65 "grammar.y"
  {
		OperandProperty prop2 = (OperandProperty)yyVals[-2+yyTop];
		OperandProperty prop4 = (OperandProperty)yyVals[0+yyTop];
		prop2.PropertyName = '<' + prop2.PropertyName + '>' + prop4.PropertyName;
		yyVal = prop2;
	}
  break;
case 7:
#line 74 "grammar.y"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 8:
#line 75 "grammar.y"
  { yyVal = new OperandProperty("^"); }
  break;
case 9:
#line 79 "grammar.y"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 10:
#line 80 "grammar.y"
  {
		OperandProperty prop1 = (OperandProperty)yyVals[-2+yyTop];
		OperandProperty prop3 = (OperandProperty)yyVals[0+yyTop];
		prop1.PropertyName += '.' + prop3.PropertyName;
		yyVal = prop1;
	}
  break;
case 11:
#line 89 "grammar.y"
  {
		AggregateOperand agg = (AggregateOperand)yyVals[0+yyTop];
		yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-2+yyTop], null, agg.AggregateType, agg.AggregatedExpression);
	}
  break;
case 12:
#line 93 "grammar.y"
  {
		AggregateOperand agg = (AggregateOperand)yyVals[0+yyTop];
		yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-5+yyTop], (CriteriaOperator)yyVals[-3+yyTop], agg.AggregateType, agg.AggregatedExpression);
	}
  break;
case 13:
#line 97 "grammar.y"
  {
		AggregateOperand agg = (AggregateOperand)yyVals[0+yyTop];
		yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-4+yyTop], null, agg.AggregateType, agg.AggregatedExpression);
	}
  break;
case 14:
#line 101 "grammar.y"
  { yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-3+yyTop], (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Exists, null); }
  break;
case 15:
#line 102 "grammar.y"
  { yyVal = JoinOperand.JoinOrAggreagate((OperandProperty)yyVals[-2+yyTop], null, Aggregate.Exists, null); }
  break;
case 18:
#line 110 "grammar.y"
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Count, null); }
  break;
case 19:
#line 111 "grammar.y"
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null); }
  break;
case 20:
#line 112 "grammar.y"
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Count, null); }
  break;
case 21:
#line 113 "grammar.y"
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)null, Aggregate.Exists, null); }
  break;
case 22:
#line 114 "grammar.y"
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Min, null); }
  break;
case 23:
#line 115 "grammar.y"
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Max, null); }
  break;
case 24:
#line 116 "grammar.y"
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Avg, null); }
  break;
case 25:
#line 117 "grammar.y"
  { yyVal = new AggregateOperand((OperandProperty)null, (CriteriaOperator)yyVals[-1+yyTop], Aggregate.Sum, null); }
  break;
case 26:
#line 121 "grammar.y"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 27:
#line 122 "grammar.y"
  {
						  string paramName = (string)yyVals[0+yyTop];
						  if(string.IsNullOrEmpty(paramName)) {
							OperandValue param = new OperandValue();
							resultParameters.Add(param);
							yyVal = param;
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
							  yyVal = p;
							  break;
							}
							if(paramNotFound) {
							  OperandParameter param = new OperandParameter(paramName);
							  resultParameters.Add(param);
							  yyVal = param;
							}
						  }
						}
  break;
case 28:
#line 148 "grammar.y"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 29:
#line 149 "grammar.y"
  { yyVal = yyVals[0+yyTop]; }
  break;
case 30:
#line 150 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Multiply ); }
  break;
case 31:
#line 151 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Divide ); }
  break;
case 32:
#line 152 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Plus ); }
  break;
case 33:
#line 153 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Minus ); }
  break;
case 34:
#line 154 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Modulo ); }
  break;
case 35:
#line 155 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.BitwiseOr ); }
  break;
case 36:
#line 156 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.BitwiseAnd ); }
  break;
case 37:
#line 157 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.BitwiseXor ); }
  break;
case 38:
#line 158 "grammar.y"
  {
								yyVal = new UnaryOperator( UnaryOperatorType.Minus, (CriteriaOperator)yyVals[0+yyTop] );
								try {
									if(yyVals[0+yyTop] is OperandValue) {
										OperandValue operand = (OperandValue)yyVals[0+yyTop];
										if(operand.Value is Int32) {
											operand.Value = -(Int32)operand.Value;
											yyVal = operand;
											break;
										} else if(operand.Value is Int64) {
											operand.Value = -(Int64)operand.Value;
											yyVal = operand;
											break;
										} else if(operand.Value is Double) {
											operand.Value = -(Double)operand.Value;
											yyVal = operand;
											break;
										} else if(operand.Value is Decimal) {
											operand.Value = -(Decimal)operand.Value;
											yyVal = operand;
											break;
										}  else if(operand.Value is Int16) {
											operand.Value = -(Int16)operand.Value;
											yyVal = operand;
											break;
										}  else if(operand.Value is SByte) {
											operand.Value = -(SByte)operand.Value;
											yyVal = operand;
											break;
										}
									}
								} catch {}
							}
  break;
case 39:
#line 191 "grammar.y"
  { yyVal = new UnaryOperator( UnaryOperatorType.Plus, (CriteriaOperator)yyVals[0+yyTop] ); }
  break;
case 40:
#line 192 "grammar.y"
  { yyVal = new UnaryOperator( UnaryOperatorType.BitwiseNot, (CriteriaOperator)yyVals[0+yyTop] ); }
  break;
case 41:
#line 193 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Equal); }
  break;
case 42:
#line 194 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.NotEqual); }
  break;
case 43:
#line 195 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Greater); }
  break;
case 44:
#line 196 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Less); }
  break;
case 45:
#line 197 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.GreaterOrEqual); }
  break;
case 46:
#line 198 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.LessOrEqual); }
  break;
case 47:
#line 199 "grammar.y"
  { yyVal = new BinaryOperator( (CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Like); }
  break;
case 48:
#line 200 "grammar.y"
  { yyVal = new UnaryOperator(UnaryOperatorType.Not, new BinaryOperator( (CriteriaOperator)yyVals[-3+yyTop], (CriteriaOperator)yyVals[0+yyTop], BinaryOperatorType.Like)); }
  break;
case 49:
#line 201 "grammar.y"
  { yyVal = new UnaryOperator(UnaryOperatorType.Not, (CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 50:
#line 202 "grammar.y"
  { yyVal = GroupOperator.And((CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 51:
#line 203 "grammar.y"
  { yyVal = GroupOperator.Or((CriteriaOperator)yyVals[-2+yyTop], (CriteriaOperator)yyVals[0+yyTop]); }
  break;
case 52:
#line 204 "grammar.y"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 53:
#line 205 "grammar.y"
  { yyVal = new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)yyVals[-2+yyTop]); }
  break;
case 54:
#line 206 "grammar.y"
  { yyVal = new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)yyVals[-3+yyTop])); }
  break;
case 55:
#line 207 "grammar.y"
  { yyVal = new InOperator((CriteriaOperator)yyVals[-2+yyTop], (IEnumerable<CriteriaOperator>)yyVals[0+yyTop]); }
  break;
case 56:
#line 208 "grammar.y"
  { yyVal = new BetweenOperator((CriteriaOperator)yyVals[-6+yyTop], (CriteriaOperator)yyVals[-3+yyTop], (CriteriaOperator)yyVals[-1+yyTop]); }
  break;
case 57:
#line 209 "grammar.y"
  { yyVal = new UnaryOperator(UnaryOperatorType.IsNull, (CriteriaOperator)yyVals[-1+yyTop]); }
  break;
case 58:
#line 210 "grammar.y"
  { yyVal = new FunctionOperator(FunctionOperatorType.IsNull, (CriteriaOperator)yyVals[-3+yyTop], (CriteriaOperator)yyVals[-1+yyTop]); }
  break;
case 59:
#line 211 "grammar.y"
  {  FunctionOperator fo = new FunctionOperator((FunctionOperatorType)yyVals[-1+yyTop], (IEnumerable<CriteriaOperator>)yyVals[0+yyTop]); lexer.CheckFunctionArgumentsCount(fo.OperatorType, fo.Operands.Count); yyVal = fo; }
  break;
case 60:
#line 212 "grammar.y"
  { yyVal = null; }
  break;
case 61:
#line 216 "grammar.y"
  { yyVal = yyVals[-1+yyTop]; }
  break;
case 62:
#line 217 "grammar.y"
  { yyVal = new List<CriteriaOperator>(); }
  break;
case 63:
#line 221 "grammar.y"
  {
							List<CriteriaOperator> lst = new List<CriteriaOperator>();
							lst.Add((CriteriaOperator)yyVals[0+yyTop]);
							yyVal = lst;
						}
  break;
case 64:
#line 226 "grammar.y"
  {
							List<CriteriaOperator> lst = (List<CriteriaOperator>)yyVals[-2+yyTop];
							lst.Add((CriteriaOperator)yyVals[0+yyTop]);
							yyVal = lst;
						}
  break;
#line default
		}
		yyTop -= yyLen[yyN];
		yyState = yyStates[yyTop];
		int yyM = yyLhs[yyN];
		if(yyState == 0 && yyM == 0) {
		  yyState = yyFinal;
		  if(yyToken < 0)
			yyToken = yyLex.advance() ? yyLex.token() : 0;
		  if(yyToken == 0)
			return yyVal;
		  goto yyLoop;
		}
		if(((yyN = yyGindex[yyM]) != 0) && ((yyN += yyState) >= 0)
			&& (yyN < yyTable.Length) && (yyCheck[yyN] == yyState))
		  yyState = yyTable[yyN];
		else
		  yyState = yyDgoto[yyM];
	 goto yyLoop;
	  }
	}
  }
   static  short [] yyLhs  = {			  -1,
	0,	0,	1,	1,	3,	3,	4,	4,	5,	5,
	6,	6,	6,	6,	6,	6,	8,	7,	7,	7,
	7,	7,	7,	7,	7,	2,	2,	2,	2,	2,
	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,
	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,
	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,
	9,	9,   10,   10,
  };
   static  short [] yyLen = {		   2,
	1,	2,	1,	3,	1,	4,	1,	1,	1,	3,
	3,	6,	5,	4,	3,	1,	1,	1,	1,	3,
	3,	4,	4,	4,	4,	1,	1,	1,	1,	3,
	3,	3,	3,	3,	3,	3,	3,	2,	2,	2,
	3,	3,	3,	3,	3,	3,	3,	4,	2,	3,
	3,	3,	3,	4,	3,	7,	4,	6,	2,	2,
	3,	2,	1,	3,
  };
   static  short [] yyDefRed = {			0,
   26,	0,	0,	0,	0,	0,	0,   27,	5,	0,
	0,	0,	0,	0,	8,	0,	0,	0,	1,	0,
	0,	0,	7,	9,	0,   29,   17,   16,	0,	0,
	0,	0,	0,	0,	0,	0,   59,   60,	0,	0,
	0,	0,   38,   39,	2,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,   21,
   20,	0,	0,	0,	0,	0,   62,	0,	0,   52,
	0,	0,	0,	0,	0,	0,   53,	0,	0,	0,
	0,	0,	0,	0,   55,	0,	0,	0,	0,	0,
	0,   30,   31,   34,   10,   11,	0,	0,   22,   23,
   24,   25,   57,	0,   61,	0,	6,	0,   54,	0,
	0,	0,	0,	0,	0,   13,	0,   58,	0,   12,
   56,
  };
  protected static  short [] yyDgoto  = {			20,
   21,   22,   23,   24,   25,   26,   27,   28,   37,   79,
  };
  protected static  int yyFinal = 20;
  protected static  short [] yySindex = {		  467,
	0,  -34,  -21,  -19,  -18,  -15,  -13,	0,	0,  -10,
   -8, 1085, 1235, -239,	0, 1235, 1235, 1235,	0,	0,
	9, 1045,	0,	0,  -42,	0,	0,	0,   -7,   -6,
 1235, 1235, 1235, 1235, 1235, 1177,	0,	0,  705, 1125,
 -240,  162,	0,	0,	0, 1235, 1235, 1235, -238, -256,
 1235, 1235, 1235, 1235, 1235, 1235, 1235,   -8,	7, 1235,
 1235, 1235, 1235, 1235, 1235, 1235, 1235,  -92, 1202,	0,
	0,  719,  769,  783,  797,  580,	0, 1045,  -41,	0,
 -224, 1045, 1112, 1125, 1235, -216,	0, 1152, 1152, 1152,
  -14,  -14,  -14,  -14,	0, 1235,   85,  178,  162,  -30,
  -30,	0,	0,	0,	0,	0,   25,  829,	0,	0,
	0,	0,	0, 1235,	0, 1235,	0, 1152,	0,  843,
 -208,   27,  867, 1045, 1235,	0, -208,	0,  907,	0,
	0,
  };
  protected static  short [] yyRindex = {			0,
	0,	1,   20,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,   10,	0,	0,   40,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,  550,
	0,  159,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,  -36,	0,	0,
	0,   11,  154,  554,	0,	0,	0,  503,  511,  519,
  263,  306,  421,  442,	0,	0,  402,  360,  382,  117,
  137,	0,	0,	0,	0,	0,   59,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,  527,	0,	0,
	0,   98,	0,  -26,	0,	0,	0,	0,	0,	0,
	0,
  };
  protected static  short [] yyGindex = {			0,
	0, 1491,	0,	6,	0,	0,  -55,	0,   17,	0,
  };
  protected static  short [] yyTable = {		   115,
   19,   15,  116,   68,   63,   29,   67,   63,   45,	3,
	4,   65,  106,   86,   64,   87,   66,   64,   30,   18,
   31,   32,   67,   62,   33,   41,   34,   65,   64,   35,
   63,   36,   66,   70,   71,   81,   85,   19,   19,   28,
  117,   19,   19,   19,   19,   19,   96,   19,   69,	2,
	3,	4,	5,	6,	7,  119,   18,   18,   15,   19,
   18,   18,   18,   18,   18,  126,   18,   46,	3,	4,
  121,  130,  127,  105,   95,	0,   28,   28,   18,   61,
   28,   28,   28,   28,   28,	0,   28,	0,	0,	0,
	0,	0,	0,   19,   19,   15,   15,   14,   28,   15,
   15,   15,   15,   15,	0,   15,	0,	0,	0,   60,
	0,	0,   18,   18,	0,	0,   33,   15,	0,	0,
	0,   67,   62,	0,   19,	0,   65,   64,	0,   63,
	0,   66,   28,   28,   14,   14,   32,	0,   14,   14,
   14,   14,   14,   18,   14,	0,	0,	0,	0,	0,
	0,   15,   15,   51,   33,	0,   14,   33,   40,   33,
   33,   33,	0,   28,	0,	2,	3,	4,	5,	6,
	7,	0,	9,	0,   32,   33,	0,   32,   61,   32,
   32,   32,   15,	0,   14,	0,	0,	0,	0,	0,
   14,   14,	0,	0,   51,   32,   40,   51,   67,   40,
	0,	0,   40,   65,   64,	0,   63,	0,   66,   33,
   33,	0,   51,	0,   67,   62,	0,   40,	0,   65,
   64,   14,   63,	0,   66,	0,	0,	0,	0,   32,
   32,	0,	0,	0,	0,	0,	0,	0,	0,	0,
   33,	0,	0,	0,	0,	0,   51,	0,	0,	0,
	0,   40,   40,	0,	0,	0,	0,	0,	0,	0,
   32,	0,   43,	0,	0,   58,   59,	0,   19,   19,
   19,   19,	0,   19,   19,   19,   19,   19,   19,   19,
   19,   19,   40,	0,	0,	0,	0,   18,   18,   18,
   18,	0,   18,   18,   18,   18,   18,   18,   18,   18,
   18,	0,	0,   43,	0,   44,   43,   28,   28,   28,
   28,	0,   28,   28,   28,   28,   28,   28,   28,   28,
   28,   43,	0,	0,	0,	0,   15,   15,   15,   15,
	0,   15,   15,   15,   15,   15,   15,   15,   15,   15,
	0,	0,	0,	0,	0,	0,   44,	0,	0,   44,
	0,	0,	0,	0,	0,   43,	0,	0,	0,   37,
	0,	0,	0,	0,   44,   14,   14,   14,   14,	0,
   14,   14,   14,   14,   14,   14,   14,   14,   14,	0,
	0,   36,	0,	0,   33,   33,   33,   33,	0,   33,
   33,   33,   33,   33,   33,   33,   33,   33,   44,	0,
   37,   35,	0,   37,   32,   32,   32,   32,	0,   32,
   32,   32,   32,   32,   32,   32,   32,   32,   37,   36,
   45,   51,   36,	0,	0,   36,   40,   40,   40,   40,
	0,   40,   40,   40,   40,   40,   40,   40,   40,   40,
   36,   46,   35,	0,	0,   35,	0,	0,	0,	0,
	0,	0,   37,   37,	0,	0,	0,	0,	0,	0,
   35,   45,	0,	0,   45,	0,   19,	0,	0,	0,
	0,	0,	0,	0,   36,   36,	0,	0,	0,   45,
	0,	0,   46,   37,	0,   46,	0,	0,	0,	0,
	0,	0,	0,	0,   35,	0,	0,	0,	0,	0,
   46,	0,   41,	0,	0,   36,   12,	0,	0,   18,
   42,   17,	0,   45,	0,	0,	0,	0,   47,	0,
	0,	0,	0,	0,	0,   35,   48,	0,	0,	0,
   43,   43,   43,   43,   46,   43,   43,   43,   43,   43,
   43,   43,	0,   41,	0,	0,   41,	0,	0,   49,
	0,   42,	0,   50,   42,	0,	0,	0,	0,   47,
   15,   41,   47,	0,	0,	0,	0,   48,	0,   42,
   48,	0,	0,   44,   44,   44,   44,   47,   44,   44,
   44,   44,   44,   44,   44,   48,	0,	0,	0,	0,
   49,	0,   16,   49,   50,   41,	0,   50,	0,	0,
	0,	0,	0,   42,	0,	0,	0,	0,   49,	0,
	0,   47,   50,	0,	0,	0,   67,   62,	0,   48,
  113,   65,   64,  114,   63,	0,   66,   37,   37,   37,
   37,	0,   37,   37,   37,   37,   37,   37,   37,   37,
   37,	0,   49,	0,	0,	0,   50,	0,	0,   36,
   36,   36,   36,	0,   36,   36,   36,   36,   36,   36,
   36,   36,   36,	0,	0,	0,	0,	0,	0,   35,
   35,   35,   35,   61,   35,   35,   35,   35,   35,   35,
   35,   35,   35,	0,	0,	0,	0,	0,   45,   45,
   45,   45,	0,   45,   45,   45,   45,   45,   45,   45,
	0,	0,	0,   60,	0,	0,	0,	0,	0,   46,
   46,   46,   46,	0,   46,   46,   46,   46,   46,   46,
   46,	0,	0,	1,	2,	3,	4,	5,	6,	7,
	8,	9,   10,   11,	0,	0,   13,	0,	0,	0,
	0,   67,   62,   14,	0,   80,   65,   64,	0,   63,
	0,   66,	0,	0,	0,   67,   62,	0,	0,  109,
   65,   64,	0,   63,	0,   66,	0,	0,	0,	0,
   41,   41,   41,   41,	0,   41,   41,   41,   42,   42,
   42,   42,	0,   42,   42,   42,   47,   47,   47,   47,
	0,   47,   47,   47,   48,   48,   48,   48,   61,   48,
   48,   48,	0,	0,	0,   67,   62,	0,	0,  110,
   65,   64,   61,   63,	0,   66,	0,   49,   49,   67,
   62,   50,   50,  111,   65,   64,	0,   63,   60,   66,
	0,	0,	0,   67,   62,	0,	0,  112,   65,   64,
	0,   63,   60,   66,	0,	0,	0,   47,   48,   49,
   50,	0,   51,   52,   53,   54,   55,   56,   57,   58,
   59,	0,   61,	0,	0,   67,   62,	0,	0,	0,
   65,   64,	0,   63,	0,   66,   61,	0,	0,   67,
   62,	0,	0,	0,   65,   64,  125,   63,	0,   66,
   61,	0,   60,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,   67,   62,	0,   60,  128,   65,   64,
	0,   63,	0,   66,	0,	0,	0,	0,	0,	0,
   60,  122,   61,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,   61,	0,	0,	0,
	0,	0,	0,   67,   62,	0,	0,  131,   65,   64,
	0,   63,   60,   66,	0,	0,	0,	0,	0,	0,
   61,	0,	0,	0,	0,	0,   60,	0,	0,	0,
	0,	0,   47,   48,   49,   50,	0,   51,   52,   53,
   54,   55,   56,   57,   58,   59,   47,   48,   49,   50,
   60,   51,   52,   53,   54,   55,   56,   57,   58,   59,
   61,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
   60,	0,	0,	0,	0,	0,   47,   48,   49,   50,
	0,   51,   52,   53,   54,   55,   56,   57,   58,   59,
   47,   48,   49,   50,	0,   51,   52,   53,   54,   55,
   56,   57,   58,   59,   47,   48,   49,   50,	0,   51,
   52,   53,   54,   55,   56,   57,   58,   59,	0,	0,
	0,   67,   62,	0,	0,	0,   65,   64,	0,   63,
	0,   66,	0,	0,	0,	0,   47,   48,   49,   50,
	0,   51,   52,   53,   54,   55,   56,   57,   58,   59,
   47,   48,   49,   50,	0,   51,   52,   53,   54,   55,
   56,   57,   58,   59,   12,   38,	0,   18,	0,   17,
	0,	0,	0,	0,   47,   48,   49,   50,   61,   51,
   52,   53,   54,   55,   56,   57,   58,   59,   67,   62,
	0,	0,	0,   65,   64,	0,   63,	0,   66,	0,
	0,   67,   62,	0,	0,	0,   65,   64,   60,   63,
	0,   66,	0,	0,   47,   48,   49,   50,   15,   51,
   52,   53,   54,   55,   56,   57,   58,   59,   67,   62,
	0,	0,	0,   65,   64,	0,   63,	0,   66,	0,
	0,	0,	0,	0,	0,   61,	0,	0,	0,	0,
   16,	0,	0,	0,	0,	0,   12,   77,   61,   18,
	0,   17,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,   60,	0,	0,	0,	0,
	0,   12,	0,	0,   18,   61,   17,	0,   60,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
   15,	0,	0,	0,   12,   60,	0,   18,	0,   17,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,  107,   15,	0,	0,	0,	0,
	0,	0,   16,	0,	0,	0,	0,	0,	0,	0,
	0,	0,   47,   48,   49,   50,	0,   51,   52,   53,
   54,   55,   56,   57,   58,   59,	0,   16,   15,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,
   10,   11,	0,	0,   13,	0,	0,	0,	0,	0,
   16,   14,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
   48,   49,   50,	0,   51,   52,   53,   54,   55,   56,
   57,   58,   59,	0,   49,   50,	0,   51,   52,   53,
   54,   55,   56,   57,   58,   59,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,   54,   55,   56,
   57,   58,   59,	1,	2,	3,	4,	5,	6,	7,
	8,	9,   10,   11,	0,	0,   13,	0,	0,	0,
	0,	0,	0,   14,	0,	0,	0,	0,	1,	2,
	3,	4,	5,	6,	7,	8,	9,   10,   11,	0,
	0,   13,	0,	0,	0,	0,	0,	0,   14,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,
   10,   11,   39,   40,   13,	0,   42,   43,   44,	0,
	0,   14,	0,	0,	0,	0,	0,	0,	0,	0,
	0,   72,   73,   74,   75,   76,   78,	0,	0,	0,
	0,	0,	0,	0,	0,	0,   82,   83,   84,	0,
	0,   88,   89,   90,   91,   92,   93,   94,	0,	0,
   97,   98,   99,  100,  101,  102,  103,  104,	0,  108,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,	0,  118,	0,	0,	0,	0,
	0,	0,	0,	0,	0,	0,  120,	0,	0,	0,
	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
	0,	0,	0,	0,  123,	0,  124,	0,	0,	0,
	0,	0,	0,	0,	0,  129,
  };
  protected static  short [] yyCheck = {			41,
	0,   94,   44,   46,   41,   40,   37,   44,	0,	0,
	0,   42,   68,  270,   41,  272,   47,   44,   40,	0,
   40,   40,   37,   38,   40,  265,   40,   42,   43,   40,
   45,   40,   47,   41,   41,  276,  275,   37,   38,	0,
  265,   41,   42,   43,   44,   45,   40,   47,   91,  258,
  259,  260,  261,  262,  263,  272,   37,   38,	0,   59,
   41,   42,   43,   44,   45,  121,   47,   59,   59,   59,
   46,  127,   46,   68,   58,   -1,   37,   38,   59,   94,
   41,   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   94,   37,   38,	0,   59,   41,
   42,   43,   44,   45,   -1,   47,   -1,   -1,   -1,  124,
   -1,   -1,   93,   94,   -1,   -1,	0,   59,   -1,   -1,
   -1,   37,   38,   -1,  124,   -1,   42,   43,   -1,   45,
   -1,   47,   93,   94,   37,   38,	0,   -1,   41,   42,
   43,   44,   45,  124,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   94,	0,   38,   -1,   59,   41,	0,   43,
   44,   45,   -1,  124,   -1,  258,  259,  260,  261,  262,
  263,   -1,  265,   -1,   38,   59,   -1,   41,   94,   43,
   44,   45,  124,   -1,  277,   -1,   -1,   -1,   -1,   -1,
   93,   94,   -1,   -1,   41,   59,   38,   44,   37,   41,
   -1,   -1,   44,   42,   43,   -1,   45,   -1,   47,   93,
   94,   -1,   59,   -1,   37,   38,   -1,   59,   -1,   42,
   43,  124,   45,   -1,   47,   -1,   -1,   -1,   -1,   93,
   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,
   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,   -1,	0,   -1,   -1,  280,  281,   -1,  268,  269,
  270,  271,   -1,  273,  274,  275,  276,  277,  278,  279,
  280,  281,  124,   -1,   -1,   -1,   -1,  268,  269,  270,
  271,   -1,  273,  274,  275,  276,  277,  278,  279,  280,
  281,   -1,   -1,   41,   -1,	0,   44,  268,  269,  270,
  271,   -1,  273,  274,  275,  276,  277,  278,  279,  280,
  281,   59,   -1,   -1,   -1,   -1,  268,  269,  270,  271,
   -1,  273,  274,  275,  276,  277,  278,  279,  280,  281,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   -1,   44,
   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,	0,
   -1,   -1,   -1,   -1,   59,  268,  269,  270,  271,   -1,
  273,  274,  275,  276,  277,  278,  279,  280,  281,   -1,
   -1,	0,   -1,   -1,  268,  269,  270,  271,   -1,  273,
  274,  275,  276,  277,  278,  279,  280,  281,   93,   -1,
   41,	0,   -1,   44,  268,  269,  270,  271,   -1,  273,
  274,  275,  276,  277,  278,  279,  280,  281,   59,   38,
	0,  268,   41,   -1,   -1,   44,  268,  269,  270,  271,
   -1,  273,  274,  275,  276,  277,  278,  279,  280,  281,
   59,	0,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   41,   -1,   -1,   44,   -1,	0,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   59,
   -1,   -1,   41,  124,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   59,   -1,	0,   -1,   -1,  124,   40,   -1,   -1,   43,
	0,   45,   -1,   93,   -1,   -1,   -1,   -1,	0,   -1,
   -1,   -1,   -1,   -1,   -1,  124,	0,   -1,   -1,   -1,
  268,  269,  270,  271,   93,  273,  274,  275,  276,  277,
  278,  279,   -1,   41,   -1,   -1,   44,   -1,   -1,	0,
   -1,   41,   -1,	0,   44,   -1,   -1,   -1,   -1,   41,
   94,   59,   44,   -1,   -1,   -1,   -1,   41,   -1,   59,
   44,   -1,   -1,  268,  269,  270,  271,   59,  273,  274,
  275,  276,  277,  278,  279,   59,   -1,   -1,   -1,   -1,
   41,   -1,  126,   44,   41,   93,   -1,   44,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   59,   -1,
   -1,   93,   59,   -1,   -1,   -1,   37,   38,   -1,   93,
   41,   42,   43,   44,   45,   -1,   47,  268,  269,  270,
  271,   -1,  273,  274,  275,  276,  277,  278,  279,  280,
  281,   -1,   93,   -1,   -1,   -1,   93,   -1,   -1,  268,
  269,  270,  271,   -1,  273,  274,  275,  276,  277,  278,
  279,  280,  281,   -1,   -1,   -1,   -1,   -1,   -1,  268,
  269,  270,  271,   94,  273,  274,  275,  276,  277,  278,
  279,  280,  281,   -1,   -1,   -1,   -1,   -1,  268,  269,
  270,  271,   -1,  273,  274,  275,  276,  277,  278,  279,
   -1,   -1,   -1,  124,   -1,   -1,   -1,   -1,   -1,  268,
  269,  270,  271,   -1,  273,  274,  275,  276,  277,  278,
  279,   -1,   -1,  257,  258,  259,  260,  261,  262,  263,
  264,  265,  266,  267,   -1,   -1,  270,   -1,   -1,   -1,
   -1,   37,   38,  277,   -1,   41,   42,   43,   -1,   45,
   -1,   47,   -1,   -1,   -1,   37,   38,   -1,   -1,   41,
   42,   43,   -1,   45,   -1,   47,   -1,   -1,   -1,   -1,
  268,  269,  270,  271,   -1,  273,  274,  275,  268,  269,
  270,  271,   -1,  273,  274,  275,  268,  269,  270,  271,
   -1,  273,  274,  275,  268,  269,  270,  271,   94,  273,
  274,  275,   -1,   -1,   -1,   37,   38,   -1,   -1,   41,
   42,   43,   94,   45,   -1,   47,   -1,  268,  269,   37,
   38,  268,  269,   41,   42,   43,   -1,   45,  124,   47,
   -1,   -1,   -1,   37,   38,   -1,   -1,   41,   42,   43,
   -1,   45,  124,   47,   -1,   -1,   -1,  268,  269,  270,
  271,   -1,  273,  274,  275,  276,  277,  278,  279,  280,
  281,   -1,   94,   -1,   -1,   37,   38,   -1,   -1,   -1,
   42,   43,   -1,   45,   -1,   47,   94,   -1,   -1,   37,
   38,   -1,   -1,   -1,   42,   43,   44,   45,   -1,   47,
   94,   -1,  124,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   37,   38,   -1,  124,   41,   42,   43,
   -1,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
  124,   93,   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   94,   -1,   -1,   -1,
   -1,   -1,   -1,   37,   38,   -1,   -1,   41,   42,   43,
   -1,   45,  124,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   94,   -1,   -1,   -1,   -1,   -1,  124,   -1,   -1,   -1,
   -1,   -1,  268,  269,  270,  271,   -1,  273,  274,  275,
  276,  277,  278,  279,  280,  281,  268,  269,  270,  271,
  124,  273,  274,  275,  276,  277,  278,  279,  280,  281,
   94,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,   -1,   -1,   -1,   -1,   -1,  268,  269,  270,  271,
   -1,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  268,  269,  270,  271,   -1,  273,  274,  275,  276,  277,
  278,  279,  280,  281,  268,  269,  270,  271,   -1,  273,
  274,  275,  276,  277,  278,  279,  280,  281,   -1,   -1,
   -1,   37,   38,   -1,   -1,   -1,   42,   43,   -1,   45,
   -1,   47,   -1,   -1,   -1,   -1,  268,  269,  270,  271,
   -1,  273,  274,  275,  276,  277,  278,  279,  280,  281,
  268,  269,  270,  271,   -1,  273,  274,  275,  276,  277,
  278,  279,  280,  281,   40,   41,   -1,   43,   -1,   45,
   -1,   -1,   -1,   -1,  268,  269,  270,  271,   94,  273,
  274,  275,  276,  277,  278,  279,  280,  281,   37,   38,
   -1,   -1,   -1,   42,   43,   -1,   45,   -1,   47,   -1,
   -1,   37,   38,   -1,   -1,   -1,   42,   43,  124,   45,
   -1,   47,   -1,   -1,  268,  269,  270,  271,   94,  273,
  274,  275,  276,  277,  278,  279,  280,  281,   37,   38,
   -1,   -1,   -1,   42,   43,   -1,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   94,   -1,   -1,   -1,   -1,
  126,   -1,   -1,   -1,   -1,   -1,   40,   41,   94,   43,
   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,   -1,   -1,   -1,   -1,
   -1,   40,   -1,   -1,   43,   94,   45,   -1,  124,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   94,   -1,   -1,   -1,   40,  124,   -1,   43,   -1,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   94,   -1,   -1,   -1,   -1,
   -1,   -1,  126,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  268,  269,  270,  271,   -1,  273,  274,  275,
  276,  277,  278,  279,  280,  281,   -1,  126,   94,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,  261,  262,  263,  264,  265,
  266,  267,   -1,   -1,  270,   -1,   -1,   -1,   -1,   -1,
  126,  277,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  269,  270,  271,   -1,  273,  274,  275,  276,  277,  278,
  279,  280,  281,   -1,  270,  271,   -1,  273,  274,  275,
  276,  277,  278,  279,  280,  281,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  276,  277,  278,
  279,  280,  281,  257,  258,  259,  260,  261,  262,  263,
  264,  265,  266,  267,   -1,   -1,  270,   -1,   -1,   -1,
   -1,   -1,   -1,  277,   -1,   -1,   -1,   -1,  257,  258,
  259,  260,  261,  262,  263,  264,  265,  266,  267,   -1,
   -1,  270,   -1,   -1,   -1,   -1,   -1,   -1,  277,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  257,  258,  259,  260,  261,  262,  263,  264,  265,
  266,  267,   12,   13,  270,   -1,   16,   17,   18,   -1,
   -1,  277,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   31,   32,   33,   34,   35,   36,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   46,   47,   48,   -1,
   -1,   51,   52,   53,   54,   55,   56,   57,   -1,   -1,
   60,   61,   62,   63,   64,   65,   66,   67,   -1,   69,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   85,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   96,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  114,   -1,  116,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  125,
  };
#line 233 "grammar.y"
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
#line default
 class Token {
  public const int CONST = 257;
  public const int AGG_EXISTS = 258;
  public const int AGG_COUNT = 259;
  public const int AGG_MIN = 260;
  public const int AGG_MAX = 261;
  public const int AGG_AVG = 262;
  public const int AGG_SUM = 263;
  public const int PARAM = 264;
  public const int COL = 265;
  public const int FN_ISNULL = 266;
  public const int FUNCTION = 267;
  public const int OR = 268;
  public const int AND = 269;
  public const int NOT = 270;
  public const int IS = 271;
  public const int NULL = 272;
  public const int OP_EQ = 273;
  public const int OP_NE = 274;
  public const int OP_LIKE = 275;
  public const int OP_GT = 276;
  public const int OP_LT = 277;
  public const int OP_GE = 278;
  public const int OP_LE = 279;
  public const int OP_IN = 280;
  public const int OP_BETWEEN = 281;
  public const int NEG = 282;
  public const int yyErrorCode = 256;
 }
 interface yyInput {
   bool advance ();
   int token ();
   Object value ();
 }
} 
