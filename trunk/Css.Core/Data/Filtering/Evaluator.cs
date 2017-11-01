namespace Css.Data.Filtering.Helpers
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.ComponentModel;
	using System.Reflection;
    using Css.Data.Filtering.Exceptions;
	using System.IO;
	using System.Collections.Generic;
	using System.Text;
#if !SL
	public interface IEvaluatorDataAccess {
		object GetValue(PropertyDescriptor descriptor, object theObject);
	}
#endif
	public class EvaluatorProperty {
		EvaluatorProperty subProperty;
		public EvaluatorProperty SubProperty {
			get {
				if(subProperty == null && PropertyPathTokenized.Length > 1) {
					subProperty = new EvaluatorProperty(string.Join(".", PropertyPathTokenized, 1, PropertyPathTokenized.Length - 1));
				}
				return subProperty;
			}
		}
		public readonly int UpDepth;
		public readonly string PropertyPath;
		string[] tokenized = null;
		public string[] PropertyPathTokenized {
			get {
				if(tokenized == null)
					tokenized = PropertyPath.Split('.');
				return tokenized;
			}
		}
		protected EvaluatorProperty(string sourcePath) {
			PropertyPath = sourcePath;
			UpDepth = 0;
			while(PropertyPath.StartsWith("^.")) {
				++UpDepth;
				PropertyPath = PropertyPath.Substring(2);
			}
		}
		public static EvaluatorProperty Create(OperandProperty property) {
			return new EvaluatorProperty(property.PropertyName);
		}
		public static bool GetIsThisProperty(string propertyName) {
			if(propertyName == null)
				return true;
			if(propertyName.Length == 0)
				return true;
			const string thisLowerString = "this";
			if(propertyName.Length == thisLowerString.Length && propertyName.ToLower() == thisLowerString)
				return true;
			return false;
		}
	}
	public class EvaluatorPropertyCache {
		Dictionary<OperandProperty, EvaluatorProperty> store = new Dictionary<OperandProperty, EvaluatorProperty>();
		public EvaluatorProperty this[OperandProperty property] {
			get {
				EvaluatorProperty result;
				if(!store.TryGetValue(property, out result)) {
					result = EvaluatorProperty.Create(property);
					store.Add(property, result);
				}
				return result;
			}
		}
	}
	public class LikeData {
		public readonly System.Text.RegularExpressions.Regex RegEx;
		public static string Escape(string autoFilterText) {
			if(autoFilterText.IndexOfAny(new char[] { '_', '%', '[' }) < 0)
				return autoFilterText;
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			StringReader reader = new StringReader(autoFilterText);
			for(; ; ) {
				int nextCharAsInt = reader.Read();
				if(nextCharAsInt == -1)
					break;
				char inputChar = (char)nextCharAsInt;
				if(inputChar == '%' || inputChar == '_' || inputChar == '[') {
					result.Append('[');
					result.Append(inputChar);
					result.Append(']');
				} else {
					result.Append(inputChar);
				}
			}
			return result.ToString();
		}
		public static string CreateStartsWithPattern(string autoFilterText) {
			return Escape(autoFilterText) + '%';
		}
		public static string CreateContainsPattern(string autoFilterText) {
			return '%' + Escape(autoFilterText) + '%';
		}
		public static string ConvertToRegEx(string originalPattern) {
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append('^');
			StringReader reader = new StringReader(originalPattern);
			for(; ; ) {
				int nextCharAsInt = reader.Read();
				if(nextCharAsInt == -1)
					break;
				char inputChar = (char)nextCharAsInt;
				switch(char.GetUnicodeCategory(inputChar)) {
					case UnicodeCategory.DecimalDigitNumber:
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.ModifierLetter:
					case UnicodeCategory.OtherLetter:
					case UnicodeCategory.TitlecaseLetter:
					case UnicodeCategory.UppercaseLetter:
						result.Append(inputChar);
						break;
					default:
						if(inputChar == '%') {
							result.Append(".*");
						} else if(inputChar == '_') {
							result.Append('.');
						} else if(inputChar == '[') {
							result.Append(inputChar);
							for(; ; ) {
								nextCharAsInt = reader.Read();
								if(nextCharAsInt == -1)
									break;
								inputChar = (char)nextCharAsInt;
								result.Append(inputChar);
								if(inputChar == ']')
									break;
							}
						} else {
							result.Append('[');
							if(inputChar == '^' || inputChar == '\\' || inputChar == '-')
								result.Append('\\');
							result.Append(inputChar);
							result.Append(']');
						}
						break;
				}
			}
			result.Append('$');
			return result.ToString();
		}
		protected LikeData(string pat, bool caseSensitive) {
			System.Text.RegularExpressions.RegexOptions options = caseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase;
			options |= System.Text.RegularExpressions.RegexOptions.Singleline;
			RegEx = new System.Text.RegularExpressions.Regex(ConvertToRegEx(pat), options);
		}
		public bool Fit(string val) {
			return RegEx.IsMatch(val);
		}
		public static LikeData Create(string pat, bool caseSensitive) {
			return new LikeData(pat, caseSensitive);
		}
		public static string UnEscape(string likePattern) {
			string result = likePattern;
			result = result.Replace("[%]", "%");
			result = result.Replace("[_]", "_");
			result = result.Replace("[[]", "[");
			return result;
		}
	}
	public class LikeDataCache {
		Dictionary<string, LikeData> store = new Dictionary<string, LikeData>();
		public LikeData this[string pattern] {
			get {
				LikeData result;
				if(!store.TryGetValue(pattern, out result)) {
					result = LikeData.Create(pattern, CaseSensitive);
					store.Add(pattern, result);
				}
				return result;
			}
		}
		public readonly bool CaseSensitive;
		public LikeDataCache(bool caseSensitive) {
			this.CaseSensitive = caseSensitive;
		}
	}
	public abstract class EvaluatorContextDescriptor {
		public abstract object GetPropertyValue(object source, EvaluatorProperty propertyPath);
		public abstract EvaluatorContext GetNestedContext(object source, string propertyPath);
		public abstract IEnumerable GetCollectionContexts(object source, string collectionName);
		public virtual bool IsTopLevelCollectionSource {
			get { return false; }
		}
	}
	public class EvaluatorContextDescriptorDefault : EvaluatorContextDescriptor {
		const bool IgnoreCaseOnDescriptors = true;
		const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.Public;
		readonly Type ReflectionType;
#if !SL
		readonly PropertyDescriptorCollection Properties;
		public IEvaluatorDataAccess DataAccess;
		public EvaluatorContextDescriptorDefault(PropertyDescriptorCollection properties, Type reflectionType) {
			this.Properties = properties;
			this.ReflectionType = reflectionType;
		}
		public EvaluatorContextDescriptorDefault(PropertyDescriptorCollection properties) : this(properties, null) { }
#endif
		public EvaluatorContextDescriptorDefault(Type reflectionType) {
			this.ReflectionType = reflectionType;
		}
		static object noResult = new object();
		object GetPropertyValue(object source, string property, bool isPath) {
			if(source == null)
				return null;
#if !SL
			if(Properties != null) {
				PropertyDescriptor pd = Properties.Find(property, IgnoreCaseOnDescriptors);
				if(pd != null) {
					if(DataAccess == null)
						return pd.GetValue(source);
					else
						return DataAccess.GetValue(pd, source);
				}
			}
#endif
			if(!isPath) {
				if(ReflectionType != null) {
					PropertyInfo pInfo = ReflectionType.GetProperty(property, ReflectionFlags);
					if(pInfo != null)
						return pInfo.GetValue(source, null);
					FieldInfo fInfo = ReflectionType.GetField(property, ReflectionFlags);
					if(fInfo != null)
						return fInfo.GetValue(source);
				}
				if(EvaluatorProperty.GetIsThisProperty(property))
					return source;
				throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, property));
			}
			return noResult;
		}
		public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
			object res = GetPropertyValue(source, propertyPath.PropertyPath, propertyPath.PropertyPathTokenized.Length > 1);
			if(res != noResult)
				return res;
			EvaluatorContext nestedContext = this.GetNestedContext(source, propertyPath.PropertyPathTokenized[0]);
			if(nestedContext == null)
				return null;
			return nestedContext.GetPropertyValue(propertyPath.SubProperty);
		}
		public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
			object nestedObject = GetPropertyValue(source, propertyPath, false);
			if(nestedObject == null)
				return null;
#if !SL
			IList list = nestedObject as IList;
			if(list == null) {
				IListSource listSource = nestedObject as IListSource;
				if(listSource != null) {
					if(listSource.ContainsListCollection)
						list = listSource.GetList();
				}
			}
			object nestedSource;
			PropertyDescriptorCollection pdc;
			if(list != null && list is ITypedList) {
				switch(list.Count) {
					case 0:
						return null;
					default:
						throw new ArgumentException("single row expected at '" + propertyPath + "', provided: " + list.Count.ToString());	
					case 1:
						break;
				}
				nestedSource = list[0];
				pdc = ((ITypedList)list).GetItemProperties(null);
			} else if(nestedObject is ITypedList) {
				nestedSource = nestedObject;
				pdc = ((ITypedList)nestedObject).GetItemProperties(null);
			} else {
				nestedSource = nestedObject;
				pdc = null;
			}
			EvaluatorContextDescriptor descriptor = new EvaluatorContextDescriptorDefault(pdc, nestedSource.GetType());
			return new EvaluatorContext(descriptor, nestedSource);
#else
			EvaluatorContextDescriptor descriptor = new EvaluatorContextDescriptorDefault(nestedObject.GetType());
			return new EvaluatorContext(descriptor, nestedObject);
#endif
		}
		public override IEnumerable GetCollectionContexts(object source, string collectionName) {
			object collectionSrc = GetPropertyValue(source, collectionName, false);
			if(collectionSrc == null)
				return null;
			IList list = collectionSrc as IList;
#if !SL
			if(list == null) {
				IListSource listSource = collectionSrc as IListSource;
				if(listSource != null)
					list = listSource.GetList();
			}
			if(list == null)
				throw new ArgumentException("not a collection: " + collectionName);
			EvaluatorContextDescriptor descriptor = null;
			ITypedList pdcSrc = list as ITypedList;
			if(pdcSrc != null) {
				descriptor = new EvaluatorContextDescriptorDefault(pdcSrc.GetItemProperties(null));
			}
			return new CollectionContexts(descriptor, list);
#else
			if(list == null)
				throw new ArgumentException("not a collection: " + collectionName);
			return new CollectionContexts(null, list);
#endif
		}
	}
	public class CollectionContexts : IEnumerable {
		public readonly EvaluatorContextDescriptor Descriptor;
		public readonly IEnumerable DataSource;
		public CollectionContexts(EvaluatorContextDescriptor descriptor, IEnumerable dataSource) {
			this.Descriptor = descriptor;
			this.DataSource = dataSource;
		}
		public virtual IEnumerator GetEnumerator() {
			return new CollectionContextsEnumerator(Descriptor, DataSource.GetEnumerator());
		}
	}
	public class CollectionContextsEnumerator : IEnumerator {
		public readonly EvaluatorContextDescriptor Descriptor;
		public readonly IEnumerator DataSource;
		public CollectionContextsEnumerator(EvaluatorContextDescriptor descriptor, IEnumerator dataSource) {
			this.Descriptor = descriptor;
			this.DataSource = dataSource;
		}
		public virtual object Current {
			get {
				EvaluatorContextDescriptor descr = Descriptor;
				if(descr == null) {
					Type objectType = typeof(object);
					if(DataSource.Current != null)
						objectType = DataSource.Current.GetType();
					descr = new EvaluatorContextDescriptorDefault(objectType);
				}
				return new EvaluatorContext(descr, DataSource.Current);
			}
		}
		public virtual bool MoveNext() {
			return DataSource.MoveNext();
		}
		public virtual void Reset() {
			DataSource.Reset();
		}
	}
	public class EvaluatorContext {
		public readonly EvaluatorContextDescriptor Descriptor;
		public readonly object Source;
		public EvaluatorContext(EvaluatorContextDescriptor descriptor, object source) {
			this.Descriptor = descriptor;
			this.Source = source;
		}
		public object GetPropertyValue(EvaluatorProperty propertyPath) {
			return Descriptor.GetPropertyValue(Source, propertyPath);
		}
		public EvaluatorContext GetNestedContext(string propertyPath) {
			return Descriptor.GetNestedContext(Source, propertyPath);
		}
		public IEnumerable GetCollectionContexts(string collectionName) {
			return Descriptor.GetCollectionContexts(Source, collectionName);
		}
	}
	public delegate object EvaluateCustomFunctionHandler(string functionName, params object[] operands);
	public class ExpressionEvaluatorCore : ExpressionEvaluatorCoreBase, IClientCriteriaVisitor {
		public ExpressionEvaluatorCore(bool caseSensitive) : base(caseSensitive) { contexts = null; }
		public ExpressionEvaluatorCore(bool caseSensitive, EvaluateCustomFunctionHandler evaluateCustomFunction) : base(caseSensitive, evaluateCustomFunction) { contexts = null; }
		EvaluatorContext[] contexts;
		protected readonly EvaluatorPropertyCache PropertyCache = new EvaluatorPropertyCache();
		protected sealed override void SetContext(EvaluatorContext context) {
			if(contexts == null) contexts = new EvaluatorContext[] { context };
			else contexts[0] = context;
		}
		protected sealed override void ClearContext() {
			contexts = null;
		}
		protected sealed override bool HasContext {
			get { return contexts != null; }
		}
		protected sealed override EvaluatorContext GetContext() {
			return contexts[0];
		}
		IEnumerable CreateNestedContext(EvaluatorProperty collectionProperty) {
			if(collectionProperty == null) {
				EvaluatorContext[] old = contexts;
				contexts = new EvaluatorContext[old.Length + 1];
				Array.Copy(old, 0, contexts, 1, old.Length);
				return contexts[1].GetCollectionContexts(null);
			}
			EvaluatorContext[] parentContext = contexts;
			contexts = new EvaluatorContext[parentContext.Length - collectionProperty.UpDepth + collectionProperty.PropertyPathTokenized.Length];
			Array.Copy(parentContext, collectionProperty.UpDepth, contexts, collectionProperty.PropertyPathTokenized.Length, contexts.Length - collectionProperty.PropertyPathTokenized.Length);
			int currentContextIndex = collectionProperty.PropertyPathTokenized.Length;
			int currentPathIndex = 0;
			while(currentContextIndex > 1) {
				EvaluatorContext currentContext = contexts[currentContextIndex];
				string currentProperty = collectionProperty.PropertyPathTokenized[currentPathIndex];
				currentContext = currentContext.GetNestedContext(currentProperty);
				if(currentContext == null)
					return null;
				--currentContextIndex;
				++currentPathIndex;
				contexts[currentContextIndex] = currentContext;
			}
			return contexts[1].GetCollectionContexts(collectionProperty.PropertyPathTokenized[collectionProperty.PropertyPathTokenized.Length - 1]);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object IClientCriteriaVisitor.Visit(AggregateOperand theOperand) {
			if(theOperand.IsTopLevel && !this.contexts[0].Descriptor.IsTopLevelCollectionSource)
				throw new InvalidOperationException("can't evaluate top level aggregate on single objct, collection property expected");	
			EvaluatorContext[] rememberedContexts = this.contexts;
			try {
				EvaluatorProperty property = theOperand.IsTopLevel ? null : PropertyCache[theOperand.CollectionProperty];
				IEnumerable nestedContextsCollection = CreateNestedContext(property);
				return DoAggregate(theOperand.AggregateType, nestedContextsCollection, theOperand.Condition, theOperand.AggregatedExpression);
			} finally {
				this.contexts = rememberedContexts;
			}
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object IClientCriteriaVisitor.Visit(JoinOperand theOperand) {
			throw new NotImplementedException(FilteringExceptionsText.ExpressionEvaluatorJoinOperandNotSupported);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object IClientCriteriaVisitor.Visit(OperandProperty theOperand) {
			EvaluatorProperty property = PropertyCache[theOperand];
			object objectResult = contexts[property.UpDepth].GetPropertyValue(property);
			if(objectResult == null)
				return null;
#if !SL
			IList list = objectResult as IList;
			if(list == null) {
				IListSource listSource = objectResult as IListSource;
				if(listSource != null) {
					if(listSource.ContainsListCollection)
						list = listSource.GetList();
				}
			}
			if(list != null && list is ITypedList) {
				switch(list.Count) {
					case 0:
						return null;
					default:
						throw new ArgumentException("single row expected at '" + theOperand.PropertyName + "', provided: " + list.Count.ToString());	
					case 1:
						return list[0];
				}
			}
#endif
			return objectResult;
		}
    }
    public abstract class ExpressionEvaluatorCoreBase : ICriteriaVisitor {
		internal static readonly Dictionary<TypeCode, Dictionary<TypeCode, TypeCode>> BinaryNumericPromotions;
		static Random random = new Random();
		static ExpressionEvaluatorCoreBase() {			
			BinaryNumericPromotions = new Dictionary<TypeCode, Dictionary<TypeCode, TypeCode>>();
			Dictionary<TypeCode, TypeCode> promotions;
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Byte, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Char, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Decimal, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Decimal);
			promotions.Add(TypeCode.Char, TypeCode.Decimal);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Decimal);
			promotions.Add(TypeCode.Int32, TypeCode.Decimal);
			promotions.Add(TypeCode.Int64, TypeCode.Decimal);
			promotions.Add(TypeCode.SByte, TypeCode.Decimal);
			promotions.Add(TypeCode.Single, TypeCode.Double);
			promotions.Add(TypeCode.UInt16, TypeCode.Decimal);
			promotions.Add(TypeCode.UInt32, TypeCode.Decimal);
			promotions.Add(TypeCode.UInt64, TypeCode.Decimal);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Double, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Double);
			promotions.Add(TypeCode.Char, TypeCode.Double);
			promotions.Add(TypeCode.Decimal, TypeCode.Double);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Double);
			promotions.Add(TypeCode.Int32, TypeCode.Double);
			promotions.Add(TypeCode.Int64, TypeCode.Double);
			promotions.Add(TypeCode.SByte, TypeCode.Double);
			promotions.Add(TypeCode.Single, TypeCode.Double);
			promotions.Add(TypeCode.UInt16, TypeCode.Double);
			promotions.Add(TypeCode.UInt32, TypeCode.Double);
			promotions.Add(TypeCode.UInt64, TypeCode.Double);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Int16, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.Int64);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Int32, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.Int64);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Int64, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int64);
			promotions.Add(TypeCode.Char, TypeCode.Int64);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int64);
			promotions.Add(TypeCode.Int32, TypeCode.Int64);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int64);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int64);
			promotions.Add(TypeCode.UInt32, TypeCode.Int64);
			promotions.Add(TypeCode.UInt64, TypeCode.Int64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.SByte, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.Int64);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.Single, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Single);
			promotions.Add(TypeCode.Char, TypeCode.Single);
			promotions.Add(TypeCode.Decimal, TypeCode.Double);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Single);
			promotions.Add(TypeCode.Int32, TypeCode.Single);
			promotions.Add(TypeCode.Int64, TypeCode.Single);
			promotions.Add(TypeCode.SByte, TypeCode.Single);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Single);
			promotions.Add(TypeCode.UInt32, TypeCode.Single);
			promotions.Add(TypeCode.UInt64, TypeCode.Single);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.UInt16, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.Int32);
			promotions.Add(TypeCode.Char, TypeCode.Int32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int32);
			promotions.Add(TypeCode.Int32, TypeCode.Int32);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int32);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.Int32);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.UInt32, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.UInt32);
			promotions.Add(TypeCode.Char, TypeCode.UInt32);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.Int64);
			promotions.Add(TypeCode.Int32, TypeCode.Int64);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.Int64);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt32);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
			promotions = new Dictionary<TypeCode, TypeCode>();
			BinaryNumericPromotions.Add(TypeCode.UInt64, promotions);
			promotions.Add(TypeCode.Byte, TypeCode.UInt64);
			promotions.Add(TypeCode.Char, TypeCode.UInt64);
			promotions.Add(TypeCode.Decimal, TypeCode.Decimal);
			promotions.Add(TypeCode.Double, TypeCode.Double);
			promotions.Add(TypeCode.Int16, TypeCode.UInt64);
			promotions.Add(TypeCode.Int32, TypeCode.UInt64);
			promotions.Add(TypeCode.Int64, TypeCode.Int64);
			promotions.Add(TypeCode.SByte, TypeCode.UInt64);
			promotions.Add(TypeCode.Single, TypeCode.Single);
			promotions.Add(TypeCode.UInt16, TypeCode.UInt64);
			promotions.Add(TypeCode.UInt32, TypeCode.UInt64);
			promotions.Add(TypeCode.UInt64, TypeCode.UInt64);
		}
		static TypeCode GetBinaryNumericPromotionCode(object left, object right, BinaryOperatorType exceptionType) {
			if(left == null || right == null)
				return TypeCode.Empty;
			return GetBinaryNumericPromotionCode(left.GetType(), right.GetType(), exceptionType, true);
		}
		internal static TypeCode GetBinaryNumericPromotionCode(Type left, Type right, BinaryOperatorType exceptionType, bool raiseException) {
			TypeCode leftTC = Type.GetTypeCode(left);
			TypeCode rightTC = Type.GetTypeCode(right);
			Dictionary<TypeCode, TypeCode> rights;
			if(!BinaryNumericPromotions.TryGetValue(leftTC, out rights)) {
				if(raiseException)
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, exceptionType.ToString(), left.FullName));
				else return TypeCode.Object;
			}
			TypeCode result;
			if(!rights.TryGetValue(rightTC, out result)) {
				if(raiseException)
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, exceptionType.ToString(), right.FullName));
				else return TypeCode.Object;
			}
			return result;
		}
		readonly LikeDataCache LikeDataCache;
		EvaluateCustomFunctionHandler evaluateCustomFunction;
		public ExpressionEvaluatorCoreBase(bool caseSensitive) : this(caseSensitive, null) { }
		public ExpressionEvaluatorCoreBase(bool caseSensitive, EvaluateCustomFunctionHandler evaluateCustomFunction) {
			this.caseSensitive = caseSensitive;
			this.LikeDataCache = new LikeDataCache(caseSensitive);
			this.evaluateCustomFunction = evaluateCustomFunction;
		}
		IComparer customComparer;
		bool caseSensitive;
		object FixValue(object value) {
			if(ReferenceEquals(value, DBNull.Value))
				return null;
			return value;
		}
		protected object Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return null;
			object value = operand.Accept(this);
			return FixValue(value);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICriteriaVisitor.Visit(BetweenOperator theOperator) {
			object val = Process(theOperator.TestExpression);
			if(Compare(val, Process(theOperator.BeginExpression)) < 0)
				return false;
			if(Compare(val, Process(theOperator.EndExpression)) > 0)
				return false;
			return true;
		}
		static Type stringType = typeof(string);
		int Compare(object left, object right, bool isEqualityCompare) {
			if (customComparer != null) return customComparer.Compare(left, right);
			if(left == null)
				return right == null ? 0 : -1;
			if(right == null)
				return 1;
			Type rightType = right.GetType();
			Type leftType = left.GetType();
			if(Object.ReferenceEquals(rightType, stringType))
				left = left.ToString();
			else
				right = ConvertValue(right, rightType, leftType);
			if(Object.ReferenceEquals(leftType, stringType))
				return System.Globalization.CultureInfo.CurrentCulture.CompareInfo.Compare((string)left, (string)right, caseSensitive ? CompareOptions.None : CompareOptions.IgnoreCase);
			else {
				if(isEqualityCompare)
					return left.Equals(right) ? 0 : -1;
				IComparable c = left as IComparable;
				if(c == null)
					throw new Exception();
				return c.CompareTo(right);
			}
		}
		protected int Compare(object left, object right) {
			return Compare(left, right, false);
		}
		object ConvertValue(object val, Type valType, Type type) {
			if(Object.ReferenceEquals(valType, type))
				return val;
			if(type.IsEnum) {
				if(Object.ReferenceEquals(valType, stringType))
					return Enum.Parse(type, (string)val, false);
				else
					return Enum.ToObject(type, val);
			}
			if(val is IConvertible)
				return Convert.ChangeType(val, type, CultureInfo.InvariantCulture);
			else
				return val;
		}
		object DoPlus(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Plus);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) + Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) + Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) + Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) + Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) + Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) + Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) + Convert.ToDecimal(right);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Plus.ToString(), tc.ToString()));
			}
		}
		object DoMinus(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Minus);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) - Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) - Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) - Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) - Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) - Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) - Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) - Convert.ToDecimal(right);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Minus.ToString(), tc.ToString()));
			}
		}
		object DoMultiply(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Multiply);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) * Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) * Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) * Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) * Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) * Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) * Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) * Convert.ToDecimal(right);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Multiply.ToString(), tc.ToString()));
			}
		}
		object DoDivide(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Divide);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) / Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) / Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) / Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) / Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) / Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) / Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) / Convert.ToDecimal(right);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Divide.ToString(), tc.ToString()));
			}
		}
		object DoModulo(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.Modulo);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) % Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) % Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) % Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) % Convert.ToUInt64(right);
				case TypeCode.Single:
					return Convert.ToSingle(left) % Convert.ToSingle(right);
				case TypeCode.Double:
					return Convert.ToDouble(left) % Convert.ToDouble(right);
				case TypeCode.Decimal:
					return Convert.ToDecimal(left) % Convert.ToDecimal(right);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.Modulo.ToString(), tc.ToString()));
			}
		}
		object DoBitwiseAnd(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.BitwiseAnd);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) & Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) & Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) & Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) & Convert.ToUInt64(right);
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.BitwiseAnd.ToString(), tc.ToString()));
			}
		}
		object DoBitwiseOr(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.BitwiseOr);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) | Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) | Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) | Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) | Convert.ToUInt64(right);
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.BitwiseOr.ToString(), tc.ToString()));
			}
		}
		object DoBitwiseXor(object left, object right) {
			TypeCode tc = GetBinaryNumericPromotionCode(left, right, BinaryOperatorType.BitwiseXor);
			if(tc == TypeCode.Empty)
				return null;
			switch(tc) {
				case TypeCode.Int32:
					return Convert.ToInt32(left) ^ Convert.ToInt32(right);
				case TypeCode.UInt32:
					return Convert.ToUInt32(left) ^ Convert.ToUInt32(right);
				case TypeCode.Int64:
					return Convert.ToInt64(left) ^ Convert.ToInt64(right);
				case TypeCode.UInt64:
					return Convert.ToUInt64(left) ^ Convert.ToUInt64(right);
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(BinaryOperator).Name, BinaryOperatorType.BitwiseXor.ToString(), tc.ToString()));
			}
		}
		static object TrueValue = true;
		static object FalseValue = false;
		static object GetBool(bool value) {
			return value ? TrueValue : FalseValue;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICriteriaVisitor.Visit(BinaryOperator theOperator) {
			object left = Process(theOperator.LeftOperand);
			object right = Process(theOperator.RightOperand);
			switch(theOperator.OperatorType) {
				case BinaryOperatorType.Like:
					if(left == null || right == null)
						return false;
					return GetBool(LikeDataCache[right.ToString()].Fit(left.ToString()));
				case BinaryOperatorType.Equal:
					return GetBool(Compare(left, right, true) == 0);
				case BinaryOperatorType.NotEqual:
					return GetBool(Compare(left, right, true) != 0);
				case BinaryOperatorType.Less:
					return GetBool(Compare(left, right) < 0);
				case BinaryOperatorType.LessOrEqual:
					return GetBool(Compare(left, right) <= 0);
				case BinaryOperatorType.Greater:
					return GetBool(Compare(left, right) > 0);
				case BinaryOperatorType.GreaterOrEqual:
					return GetBool(Compare(left, right) >= 0);
				case BinaryOperatorType.Plus:
					if(left is string || right is string) {
						return string.Format(CultureInfo.InvariantCulture, "{0}{1}", left, right);
					}
					return DoPlus(left, right);
				case BinaryOperatorType.Minus:
					return DoMinus(left, right);
				case BinaryOperatorType.Multiply:
					return DoMultiply(left, right);
				case BinaryOperatorType.Divide:
					return DoDivide(left, right);
				case BinaryOperatorType.Modulo:
					return DoModulo(left, right);
				case BinaryOperatorType.BitwiseAnd:
					return DoBitwiseAnd(left, right);
				case BinaryOperatorType.BitwiseOr:
					return DoBitwiseOr(left, right);
				case BinaryOperatorType.BitwiseXor:
					return DoBitwiseXor(left, right);
				default:
					throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(BinaryOperator).Name, theOperator.OperatorType.ToString()));
			}
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICriteriaVisitor.Visit(InOperator theOperator) {
			object val = Process(theOperator.LeftOperand);
			foreach(CriteriaOperator op in theOperator.Operands)
				if(Compare(val, Process(op), true) == 0)
					return GetBool(true);
			return GetBool(false);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICriteriaVisitor.Visit(GroupOperator theOperator) {
			bool res = theOperator.OperatorType == GroupOperatorType.And;
			int count = theOperator.Operands.Count;
			for(int i = 0; i < count; i++)
				if(res != (bool)Process((CriteriaOperator)theOperator.Operands[i]))
					return GetBool(!res);
			return GetBool(res);
		}
		string CalcStringArgument(object operand) {
			object obj = Process((CriteriaOperator)operand);
			if(obj == null)
				return null;
			return obj.ToString();
		}
		string GetFirstArgumentAsString(FunctionOperator op) {
			return CalcStringArgument(op.Operands[0]);
		}
		string GetStringFromNumber(object number,string strOperator,string strOperatorType) {
			switch(Type.GetTypeCode(number.GetType())) {
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single: {
					return number.ToString();
				}
				default: {
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, strOperator, strOperatorType, number.ToString()));
				}
			}
		}
		long GetLongArgument(object value, string strOperator, string strOperatorType) {
			switch(Type.GetTypeCode(value.GetType())) {
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single: {
						return Convert.ToInt64(value);
					}
				default: {
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, strOperator, strOperatorType, value.ToString()));
					}
			}
		}
		int GetIntArgument(object value, string strOperator, string strOperatorType) {
			switch(Type.GetTypeCode(value.GetType())) {
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single: {
						return Convert.ToInt32(value);
					}
				default: {
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, strOperator, strOperatorType, value.ToString()));
					}
			}
		}
		DateTime GetDateTimeArgument(object value, string strOperator, string strOperatorType) {
			if(!(value is DateTime)) {
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, strOperator, strOperatorType, value.ToString()));
			}
			return (DateTime)value;
		}
		TimeSpan GetTimeSpanArgument(object value, string strOperator, string strOperatorType) {
			if(!(value is TimeSpan)) {
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, strOperator, strOperatorType, value.ToString()));
			}
			return (TimeSpan)value;
		}
		double GetDoubleArgument(object value, string strOperator ,string strOperatorType) {
			switch(Type.GetTypeCode(value.GetType())) {
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single: {
					return Convert.ToDouble(value);
				}
				default: {
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, strOperator, strOperatorType, value.ToString()));
				}
			}
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICriteriaVisitor.Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.Abs:
					return FnAbs(theOperator);
				case FunctionOperatorType.Round:
					return FnRound(theOperator);
				case FunctionOperatorType.Sign:
					return FnSign(theOperator);
				case FunctionOperatorType.Floor:
					return FnFloor(theOperator);
				case FunctionOperatorType.Ceiling:
					return FnCeiling(theOperator);
				case FunctionOperatorType.Sqr:
					return FnSqr(theOperator);
				case FunctionOperatorType.Log:
					return FnLog(theOperator);
				case FunctionOperatorType.Rnd:
					return FnRnd();
				case FunctionOperatorType.Sin:
					return FnSin(theOperator);
				case FunctionOperatorType.Tan:
					return FnTan(theOperator);
				case FunctionOperatorType.Atn:
					return FnAtan(theOperator);
				case FunctionOperatorType.Acos:
					return FnAcos(theOperator);
				case FunctionOperatorType.Asin:
					return FnAsin(theOperator);
				case FunctionOperatorType.Atn2:
					return FnAtn2(theOperator);
#if !(CF || SL)
				case FunctionOperatorType.BigMul:
					return FnBigMul(theOperator);
#endif
				case FunctionOperatorType.Cosh:
					return FnCosh(theOperator);
				case FunctionOperatorType.Log10:
					return FnLog10(theOperator);
				case FunctionOperatorType.Sinh:
					return FnSinh(theOperator);
				case FunctionOperatorType.Tanh:
					return FnTanh(theOperator);
				case FunctionOperatorType.Cos:
					return FnCos(theOperator);
				case FunctionOperatorType.Exp:
					return FnExp(theOperator);
				case FunctionOperatorType.Power:
					return FnPower(theOperator);
				case FunctionOperatorType.Len:
					return FnLen(theOperator);
				case FunctionOperatorType.Trim:
					return FnTrim(theOperator);
				case FunctionOperatorType.Concat:
					return FnConcat(theOperator);
				case FunctionOperatorType.ToStr:
					return FnStr(theOperator);
				case FunctionOperatorType.Ascii:
					return FnAscii(theOperator);
				case FunctionOperatorType.Char:
					return FnChar(theOperator);
				case FunctionOperatorType.Upper:
					return FnUpper(theOperator);
				case FunctionOperatorType.Lower:
					return FnLower(theOperator);
				case FunctionOperatorType.Iif:
					return Process((CriteriaOperator)theOperator.Operands[((bool)Process((CriteriaOperator)theOperator.Operands[0])) ? 1 : 2]);
				case FunctionOperatorType.IsNull:
					return FnIsNull(theOperator);
				case FunctionOperatorType.IsNullOrEmpty:
					return FnIsNullOrEmpty(theOperator);
				case FunctionOperatorType.Substring:
					return FnSubstring(theOperator);
				case FunctionOperatorType.Replace:
					return FnReplace(theOperator);
				case FunctionOperatorType.Reverse:
					return FnReverse(theOperator);
				case FunctionOperatorType.Insert:
					return FnInsert(theOperator);
				case FunctionOperatorType.CharIndex:
					return FnCharIndex(theOperator);
				case FunctionOperatorType.PadLeft:
					return FnPadLeft(theOperator);
				case FunctionOperatorType.PadRight:
					return FnPadRight(theOperator);
				case FunctionOperatorType.Remove:
					return FnRemove(theOperator);
				case FunctionOperatorType.LocalDateTimeThisYear:
				case FunctionOperatorType.LocalDateTimeThisMonth:
				case FunctionOperatorType.LocalDateTimeLastWeek:
				case FunctionOperatorType.LocalDateTimeThisWeek:
				case FunctionOperatorType.LocalDateTimeYesterday:
				case FunctionOperatorType.LocalDateTimeToday:
				case FunctionOperatorType.LocalDateTimeNow:
				case FunctionOperatorType.LocalDateTimeTomorrow:
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
				case FunctionOperatorType.LocalDateTimeNextWeek:
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
				case FunctionOperatorType.LocalDateTimeNextMonth:
				case FunctionOperatorType.LocalDateTimeNextYear:
					if(theOperator.Operands.Count != 0)
						throw new ArgumentException("theOperator.Operands.Count != 0");
					return EvaluateLocalDateTime(theOperator.OperatorType);
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
					return Process(ExpandIsOutlookInterval(theOperator));
				case FunctionOperatorType.GetDate:
					return FnGetDate(theOperator);
				case FunctionOperatorType.GetMilliSecond:
					return FnGetMilliSecond(theOperator);
				case FunctionOperatorType.GetSecond:
					return FnGetSecond(theOperator);
				case FunctionOperatorType.GetMinute:
					return FnGetMinute(theOperator);
				case FunctionOperatorType.GetHour:
					return FnGetHour(theOperator);
				case FunctionOperatorType.GetDay:
					return FnGetDay(theOperator);
				case FunctionOperatorType.GetMonth:
					return FnGetMonth(theOperator);
				case FunctionOperatorType.GetYear:
					return FnGetYear(theOperator);
				case FunctionOperatorType.GetTimeOfDay:
					return FnGetTimeOfDay(theOperator);
				case FunctionOperatorType.GetDayOfWeek:
					return FnGetDayOfWeek(theOperator);
				case FunctionOperatorType.GetDayOfYear:
					return FnGetDayOfYear(theOperator);
				case FunctionOperatorType.AddTimeSpan:
					return FnAddTimeSpan(theOperator);
				case FunctionOperatorType.AddTicks:
					return FnAddTicks(theOperator);
				case FunctionOperatorType.AddMilliSeconds:
					return FnAddMilliSeconds(theOperator);
				case FunctionOperatorType.AddSeconds:
					return FnAddSeconds(theOperator);
				case FunctionOperatorType.AddMinutes:
					return FnAddMinutes(theOperator);
				case FunctionOperatorType.AddHours:
					return FnAddHours(theOperator);
				case FunctionOperatorType.AddDays:
					return FnAddDays(theOperator);
				case FunctionOperatorType.AddMonths:
					return FnAddMounths(theOperator);
				case FunctionOperatorType.AddYears:
					return FnAddYears(theOperator);
				case FunctionOperatorType.Now:
					return DateTime.Now;
				case FunctionOperatorType.UtcNow:
					return DateTime.UtcNow;
				case FunctionOperatorType.Today:
					return DateTime.Today;
				case FunctionOperatorType.Custom:
					return FnCustom(theOperator);
				default:
					throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
			}
		}
		object FnCustom(FunctionOperator theOperator) {
			if (evaluateCustomFunction == null) throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
			if (!(theOperator.Operands[0] is OperandValue) || (((OperandValue)theOperator.Operands[0]).Value == null) || !(((OperandValue)theOperator.Operands[0]).Value is string)) {
				throw new ArgumentException("Custom function name not found.");
			}
			string functionName = (string)((OperandValue)theOperator.Operands[0]).Value;
			if (theOperator.Operands.Count > 1) {
				object[] operands = new object[theOperator.Operands.Count - 1];
				for (int i = 1; i < theOperator.Operands.Count; i++) {
					operands[i - 1] = Process((CriteriaOperator)theOperator.Operands[i]);
				}
				return evaluateCustomFunction(functionName, operands);
			}
			return evaluateCustomFunction(functionName);
		}
		object FnPadRight(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if (str == null) { return null; }
			object op2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if (op2 == null) { return null; }
			switch (Type.GetTypeCode(op2.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.SByte:
				case TypeCode.Byte:
				break;
				default:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.PadRight.ToString(), op2.ToString()));
			}
			int totalWidth = Convert.ToInt32(op2);
			if (theOperator.Operands.Count >= 3) {
				object op3 = Process((CriteriaOperator)theOperator.Operands[2]);
				if (op3 == null) { return null; }
				if (op3 is char) {
					return str.PadRight(totalWidth, (char)op3);
				} else if (op3 is string && ((string)op3).Length > 0) {
					return str.PadRight(totalWidth, ((string)op3)[0]);
				}
			}
			return str.PadRight(totalWidth);
		}
		object FnPadLeft(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if (str == null) { return null; }
			object op2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if (op2 == null) { return null; }
			switch (Type.GetTypeCode(op2.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.SByte:
				case TypeCode.Byte:
				break;
				default:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.PadLeft.ToString(), op2.ToString()));
			}
			int totalWidth = Convert.ToInt32(op2);
			if (theOperator.Operands.Count >= 3) {
				object op3 = Process((CriteriaOperator)theOperator.Operands[2]);
				if (op3 == null) { return null; }
				if (op3 is char) {
					return str.PadLeft(totalWidth, (char)op3);
				} else if (op3 is string && ((string)op3).Length > 0) {
					return str.PadLeft(totalWidth, ((string)op3)[0]);
				}
			}
			return str.PadLeft(totalWidth);
		}
		object FnTanh(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if (op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Tanh.ToString());
			result = Math.Tanh(value);
			return result;
		}
		object FnSinh(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if (op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Sinh.ToString());
			result = Math.Sinh(value);
			return result;
		}
		object FnLog10(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if (op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Log10.ToString());
			result = Math.Log10(value);
			return result;
		}
		object FnCosh(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if (op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Cosh.ToString());
			result = Math.Cosh(value);
			return result;
		}
		object FnAsin(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if (op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Asin.ToString());
			result = Math.Asin(value);
			return result;
		}
#if !(CF || SL)
		object FnBigMul(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if (op == null) { return null; }
			object op2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if (op2 == null) { return null; }
			switch (Type.GetTypeCode(op.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.SByte:
				case TypeCode.Byte:
				break;
				default:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.BigMul.ToString(), op.ToString()));
			}
			switch (Type.GetTypeCode(op.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.SByte:
				case TypeCode.Byte:
				break;
				default:
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.BigMul.ToString(), op2.ToString()));
			}
			return Math.BigMul(Convert.ToInt32(op), Convert.ToInt32(op2));
		}
#endif
		object FnAtn2(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if (op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Atn2.ToString());
			object op2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if (op2 == null) { result = null; }
			double value2 = GetDoubleArgument(op2, typeof(FunctionOperator).Name, FunctionOperatorType.Atn2.ToString());
			result = Math.Atan2(value, value2);
			return result;
		}
		object FnAcos(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if (op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Acos.ToString());
			result = Math.Acos(value);
			return result;
		}
		object FnAddTimeSpan(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			TimeSpan ts = GetTimeSpanArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			return dt.Add(ts);
		}
		object FnAddYears(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			return dt.AddYears(GetIntArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
		}
		object FnAddMounths(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			return dt.AddMonths(GetIntArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
		}
		object FnAddDays(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			return dt.AddDays(GetDoubleArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
		}
		object FnAddHours(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			return dt.AddHours(GetDoubleArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
		}
		object FnAddSeconds(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			return dt.AddSeconds(GetDoubleArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
		}
		object FnAddMinutes(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			return dt.AddMinutes(GetDoubleArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
		}
		object FnAddTicks(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			return dt.AddTicks(GetLongArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));
		}
		object FnAddMilliSeconds(FunctionOperator theOperator) {
			object ob1 = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob1 == null)
				return null;
			DateTime dt = GetDateTimeArgument(ob1, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString());
			object ob2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(ob2 == null) {
				return null;
			}
			return dt.AddMilliseconds(GetDoubleArgument(ob2, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()));			
		}
		object FnGetDate(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).Date;
		}
		object FnGetMilliSecond(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).Millisecond;
		}
		object FnGetSecond(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).Second;
		}
		object FnGetMinute(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).Minute;
		}
		object FnGetHour(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).Hour;
		}
		object FnGetDay(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).Day;
		}
		object FnGetMonth(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).Month;
		}
		object FnGetYear(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).Year;
		}
		object FnGetTimeOfDay(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).TimeOfDay.Ticks;
		}
		object FnGetDayOfWeek(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;			
			return (int)GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).DayOfWeek;
		}
		object FnGetDayOfYear(FunctionOperator theOperator) {
			object ob = Process((CriteriaOperator)theOperator.Operands[0]);
			if(ob == null)
				return null;
			return GetDateTimeArgument(ob, typeof(FunctionOperator).Name, theOperator.OperatorType.ToString()).DayOfYear;
		}
		object FnCeiling(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { return null; }
			switch(Type.GetTypeCode(op.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Byte:
					return op;
				case TypeCode.Decimal:
#if CF || SL
					return (Decimal)Math.Ceiling(Convert.ToDouble(op));
#else
					return Math.Ceiling((Decimal)op);
#endif
				case TypeCode.Double:
					return Math.Ceiling((double)op);
				case TypeCode.Single:
					return (Single)Math.Ceiling((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Ceiling.ToString(), op.ToString()));
			}
		}
		object FnFloor(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { return null; }
			switch(Type.GetTypeCode(op.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Byte:
					return op;
				case TypeCode.Decimal:
#if CF || SL
					return (Decimal)Math.Floor(Convert.ToDouble(op));
#else
					return Math.Floor((Decimal)op);
#endif
				case TypeCode.Double:
					return Math.Floor((double)op);
				case TypeCode.Single:
					return (Single)Math.Floor((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Floor.ToString(), op.ToString()));
			}
		}
		object FnSign(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { return null; }
			switch(Type.GetTypeCode(op.GetType())) {
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return 1;
				case TypeCode.Decimal:
					return (Decimal)Math.Sign((Decimal)op);
				case TypeCode.Double:
					return (double)Math.Sign((double)op);
				case TypeCode.Int16:
					return (Int16)Math.Sign((Int16)op);
				case TypeCode.Int32:
					return (Int32)Math.Sign((Int32)op);
				case TypeCode.Int64:
					return (Int64)Math.Sign((Int64)op);
				case TypeCode.SByte:
					return (sbyte)Math.Sign((sbyte)op);
				case TypeCode.Single:
					return (Single)Math.Sign((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Sign.ToString(), op.ToString()));
			}
		}
		object FnRound(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { return null; }
			switch(Type.GetTypeCode(op.GetType())) {
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Byte:
					return op;
				case TypeCode.Decimal:
#if CF || SL
					return (Decimal)Math.Round(Convert.ToDouble(op));
#else
					return Math.Round((Decimal)op);
#endif
				case TypeCode.Double:
					return Math.Round((double)op);
				case TypeCode.Single:
					return (Single)Math.Round((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Round.ToString(), op.ToString()));
			}
		}
		object FnRemove(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if (str == null) { return null; }
			int op2 = Convert.ToInt32(Process((CriteriaOperator)theOperator.Operands[1]));
#if CF || SL
			int op3 = Convert.ToInt32(Process((CriteriaOperator)theOperator.Operands[2]));
			return str.Remove(op2, op3);
#else
			if (theOperator.Operands.Count >= 3) {
				int op3 = Convert.ToInt32(Process((CriteriaOperator)theOperator.Operands[2]));
				return str.Remove(op2, op3);
			}
			return str.Remove(op2);
#endif
		}
		object FnInsert(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if (str == null) { return null; }
			StringBuilder strBilder = new StringBuilder(str);
			int op2 = Convert.ToInt32(Process((CriteriaOperator)theOperator.Operands[1]));
			string op3 = Process((CriteriaOperator)theOperator.Operands[2]).ToString();
			return str.Insert(op2, op3);
		}
		object FnReverse(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if(str == null) { return null; }
			return Reverse(str);
		}
		public string Reverse(string str) {
			int len = str.Length;
			char[] arr = new char[len];
			for(int i = 0; i < len; i++) {
				arr[i] = str[len - 1 - i];
			}
			return new string(arr);
		}
		object FnReplace(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if(str == null) { return null; }
			string op2 = Process((CriteriaOperator)theOperator.Operands[1]).ToString();
			if(op2 == null) { return null; }
			string op3 = Process((CriteriaOperator)theOperator.Operands[2]).ToString();
			if(op3 == null) { return null; }
			return str.Replace(op2, op3);
		}
		object FnSubstring(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if(str == null) { return null; }
			object op2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if(op2 == null) { return null; }
			int iop2 = (int)op2;
			object op3 = null;
			if(theOperator.Operands.Count >= 3) {
				op3 = Process((CriteriaOperator)theOperator.Operands[2]);
			}
			if(op3 == null) {
				return str.Substring(iop2);
			}
			else {
				return str.Substring(iop2, (int)op3);
			}
		}
		object FnCharIndex(FunctionOperator theOperator) {
			string str1 = GetFirstArgumentAsString(theOperator);
			if(str1 == null) { return null; }
			string str2 = Process((CriteriaOperator)theOperator.Operands[1]).ToString();
			if(str2 == null) { return null; }
			object op3 = null;
			if(theOperator.Operands.Count >= 3) {
				op3 = Process((CriteriaOperator)theOperator.Operands[2]);
			}
			object op4 = null;
			if (theOperator.Operands.Count >= 4) {
				op4 = Process((CriteriaOperator)theOperator.Operands[3]);
			}
			if(op3 == null) {
				return str2.IndexOf(str1);
			} else if (op4 == null) {
				return str2.IndexOf(str1, Convert.ToInt32(op3));
			} else {
				return str2.IndexOf(str1, Convert.ToInt32(op3), Convert.ToInt32(op4));
			}
		}
		object FnIsNull(FunctionOperator theOperator) {
			if(theOperator.Operands.Count == 1) {
				return Process((CriteriaOperator)theOperator.Operands[0]) == null;
			}
			else {
				foreach(CriteriaOperator op in theOperator.Operands) {
					object obj = Process(op);
					if(obj != null) { return obj; }
				}
				return null;
			}
		}
		object FnIsNullOrEmpty(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) return true;
			return string.IsNullOrEmpty(op.ToString());
		}
		object FnStr(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { return null; }
			return GetStringFromNumber(op, typeof(FunctionOperator).Name, FunctionOperatorType.ToStr.ToString());
		}
		object FnAscii(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if(string.IsNullOrEmpty(str)) { return null; }
			return (int)str[0];
		}
		object FnChar(FunctionOperator theOperator) {
			object oper1 = Process((CriteriaOperator)theOperator.Operands[0]);
			switch(Type.GetTypeCode(oper1.GetType())) {
				case TypeCode.Byte:
				case TypeCode.Decimal:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.SByte: {
					return ((Char)Convert.ToInt64(oper1)).ToString();
				}
				default: {
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Char.ToString(), oper1.ToString()));
				}
			}
		}
		object FnLower(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if(str == null) { return null; }
			return str.ToLower(CultureInfo.InvariantCulture);
		}
		object FnUpper(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if(str == null) { return null; }
			return str.ToUpper(CultureInfo.InvariantCulture);
		}
		string FnConcat(FunctionOperator theOperator) {
			string res = null;
			foreach(CriteriaOperator op in theOperator.Operands) {
				string val = CalcStringArgument(op);
				if(val == null) {
					res = null;
					break;
				}
				res += val;
			}
			return res;
		}
		object FnTrim(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if(str == null) { return null; }
			return str.Trim();
		}
		object FnLen(FunctionOperator theOperator) {
			string str = GetFirstArgumentAsString(theOperator);
			if(str == null) { return null; }
			return str.Length;
		}
		object FnPower(FunctionOperator theOperator) {
			object oper1 = Process((CriteriaOperator)theOperator.Operands[0]);
			object oper2 = Process((CriteriaOperator)theOperator.Operands[1]);
			if (oper1 == null || oper2 == null) { return null; }
			double value1 = GetDoubleArgument(oper1, typeof(FunctionOperator).Name, FunctionOperatorType.Power.ToString());
			double value2 = GetDoubleArgument(oper2, typeof(FunctionOperator).Name, FunctionOperatorType.Power.ToString());
			return Math.Pow(value1, value2);;
		}
		object FnExp(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Exp.ToString());
			result = Math.Exp(value);
			return result;
		}
		object FnCos(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Cos.ToString());
			result = Math.Cos(value);
			return result;
		}
		object FnAtan(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Atn.ToString());
			result = Math.Atan(value);
			return result;
		}
		object FnTan(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Tan.ToString());
			result = Math.Tan(value);
			return result;
		}
		object FnSin(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Sin.ToString());
			result = Math.Sin(value);
			return result;
		}
		static object FnRnd() {
			lock(random) {
				return random.NextDouble();
			}
		}
		object FnLog(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { return null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Log.ToString());
#if !(CF || SL)
			if (theOperator.Operands.Count >= 2) {
				object op2 = Process((CriteriaOperator)theOperator.Operands[1]);
				if(op2 == null) { return null; }
				double value2 = GetDoubleArgument(op2, typeof(FunctionOperator).Name, FunctionOperatorType.Log.ToString());
				return Math.Log(value, value2);
			}
#endif
			return Math.Log(value);
		}
		object FnSqr(FunctionOperator theOperator) {
			object result;
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { result = null; }
			double value = GetDoubleArgument(op, typeof(FunctionOperator).Name, FunctionOperatorType.Sqr.ToString());
			if(value < 0) {
				throw new ArgumentException("Value can't be negative.");
			}
			result = Math.Sqrt(value);
			return result;
		}
		object FnAbs(FunctionOperator theOperator) {
			object op = Process((CriteriaOperator)theOperator.Operands[0]);
			if(op == null) { return null; }
			switch(Type.GetTypeCode(op.GetType())) {
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return op;
				case TypeCode.Decimal:
					return Math.Abs((Decimal)op);
				case TypeCode.Double:
					return Math.Abs((double)op);
				case TypeCode.Int16:
					return Math.Abs((Int16)op);
				case TypeCode.Int32:
					return Math.Abs((Int32)op);
				case TypeCode.Int64:
					return Math.Abs((Int64)op);
				case TypeCode.SByte:
					return Math.Abs((sbyte)op);
				case TypeCode.Single:
					return Math.Abs((Single)op);
				default:
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(FunctionOperator).Name, FunctionOperatorType.Abs.ToString(), op.ToString()));
			}
		}
		static CriteriaOperator MakeTypicalOutlookInterval(CriteriaOperator op, FunctionOperatorType lowerBound, FunctionOperatorType upperBound) {
			return op >= new FunctionOperator(lowerBound) & op < new FunctionOperator(upperBound);
		}
		public static CriteriaOperator ExpandIsOutlookInterval(FunctionOperator theOperator) {
			if(theOperator.Operands.Count != 1)
				throw new ArgumentException("theOperator.Operands.Count != 1");
			CriteriaOperator op = theOperator.Operands[0];
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
					return op >= new FunctionOperator(FunctionOperatorType.LocalDateTimeNextYear);
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeNextMonth, FunctionOperatorType.LocalDateTimeNextYear);
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeTwoWeeksAway, FunctionOperatorType.LocalDateTimeNextMonth);
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeNextWeek, FunctionOperatorType.LocalDateTimeTwoWeeksAway);
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeDayAfterTomorrow, FunctionOperatorType.LocalDateTimeNextWeek);
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeTomorrow, FunctionOperatorType.LocalDateTimeDayAfterTomorrow);
				case FunctionOperatorType.IsOutlookIntervalToday:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeToday, FunctionOperatorType.LocalDateTimeTomorrow);
				case FunctionOperatorType.IsOutlookIntervalYesterday:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeYesterday, FunctionOperatorType.LocalDateTimeToday);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisWeek, FunctionOperatorType.LocalDateTimeYesterday);
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeLastWeek, FunctionOperatorType.LocalDateTimeThisWeek);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisMonth, FunctionOperatorType.LocalDateTimeLastWeek);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
					return MakeTypicalOutlookInterval(op, FunctionOperatorType.LocalDateTimeThisYear, FunctionOperatorType.LocalDateTimeThisMonth);
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
					return op < new FunctionOperator(FunctionOperatorType.LocalDateTimeThisYear);
				default:
					throw new InvalidOperationException("theOperator.OperatorType is not IsOutlookInterval* or internal error");
			}
		}
		public static DateTime GetWeekStart(DateTime now) {
			return GetWeekStart(now, System.Globalization.DateTimeFormatInfo.CurrentInfo);
		}
		public static DateTime GetWeekStart(DateTime now, DateTimeFormatInfo dtfi) {
			DateTime today = now.Date;
			DayOfWeek current = today.DayOfWeek;
			DayOfWeek wanted = dtfi.FirstDayOfWeek;
			int diff = (((int)current) - ((int)wanted)) % 7;
			return today.AddDays(-diff);
		}
		public static DateTime EvaluateLocalDateTime(FunctionOperatorType type) {
			DateTime now = DateTime.Now;
			switch(type) {
				case FunctionOperatorType.LocalDateTimeThisYear:
					return new DateTime(now.Year, 1, 1);
				case FunctionOperatorType.LocalDateTimeThisMonth:
					return new DateTime(now.Year, now.Month, 1);
				case FunctionOperatorType.LocalDateTimeLastWeek:
					return GetWeekStart(now).AddDays(-7);
				case FunctionOperatorType.LocalDateTimeThisWeek:
					return GetWeekStart(now);
				case FunctionOperatorType.LocalDateTimeYesterday:
					return now.Date.AddDays(-1);
				case FunctionOperatorType.LocalDateTimeToday:
					return now.Date;
				case FunctionOperatorType.LocalDateTimeNow:
					return now;
				case FunctionOperatorType.LocalDateTimeTomorrow:
					return now.Date.AddDays(1);
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
					return now.Date.AddDays(2);
				case FunctionOperatorType.LocalDateTimeNextWeek:
					return GetWeekStart(now).AddDays(7);
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
					return GetWeekStart(now).AddDays(14);
				case FunctionOperatorType.LocalDateTimeNextMonth:
					return new DateTime(now.Year, now.Month, 1).AddMonths(1);
				case FunctionOperatorType.LocalDateTimeNextYear:
					return new DateTime(now.Year + 1, 1, 1);
				default:
					throw new InvalidOperationException("theOperator.OperatorType is not LocalDateTime* or internal error");
			}
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICriteriaVisitor.Visit(OperandValue theOperand) {
			object operandValue = theOperand.Value;
			return operandValue;
		}
		object UnaryNumericPromotions(object operand) {
			switch(Type.GetTypeCode(operand.GetType())) {
				case TypeCode.SByte:
					return (int)(SByte)operand;
				case TypeCode.Byte:
					return (int)(Byte)operand;
				case TypeCode.Int16:
					return (int)(Int16)operand;
				case TypeCode.UInt16:
					return (int)(UInt16)operand;
				case TypeCode.Char:
					return (int)(Char)operand;
				default:
					return operand;
			}
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        object ICriteriaVisitor.Visit(UnaryOperator theOperator) {
			object operand = Process(theOperator.Operand);
			object converted;
			switch(theOperator.OperatorType) {
				case UnaryOperatorType.IsNull:
					return operand == null;
				case UnaryOperatorType.Not:
					if(operand == null)
						return null;
					return GetBool(!(bool)operand);
				case UnaryOperatorType.Plus:
					if(operand == null)
						return null;
					converted = UnaryNumericPromotions(operand);
					switch(Type.GetTypeCode(converted.GetType())) {
						case TypeCode.Decimal:
						case TypeCode.Double:
						case TypeCode.Int32:
						case TypeCode.Int64:
						case TypeCode.Single:
						case TypeCode.UInt32:
						case TypeCode.UInt64:
							return converted;
						default:
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(UnaryOperator).Name, UnaryOperatorType.Plus.ToString(), operand.GetType().FullName));
					}
				case UnaryOperatorType.Minus:
					if(operand == null)
						return null;
					converted = (operand is UInt32) ? (Int64)(UInt32)operand : UnaryNumericPromotions(operand);
					switch(Type.GetTypeCode(converted.GetType())) {
						case TypeCode.Int32:
							return -(Int32)converted;
						case TypeCode.Int64:
							return -(Int64)converted;
						case TypeCode.Single:
							return -(Single)converted;
						case TypeCode.Double:
							return -(Double)converted;
						case TypeCode.Decimal:
							return -(Decimal)converted;
						default:
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(UnaryOperator).Name, UnaryOperatorType.Minus.ToString(), operand.GetType().FullName));
					}
				case UnaryOperatorType.BitwiseNot:
					if(operand == null)
						return null;
					converted = UnaryNumericPromotions(operand);
					switch(Type.GetTypeCode(converted.GetType())) {
						case TypeCode.Int32:
							return ~(Int32)converted;
						case TypeCode.Int64:
							return ~(Int64)converted;
						case TypeCode.UInt32:
							return ~(UInt32)converted;
						case TypeCode.UInt64:
							return ~(UInt64)converted;
						default:
							throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(UnaryOperator).Name, UnaryOperatorType.BitwiseNot.ToString(), operand.GetType().FullName));
					}
				default:
					throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(UnaryOperator).Name, theOperator.OperatorType.ToString()));
			}
		}
		abstract class AggregateProcessingParam {
			public readonly ExpressionEvaluatorCoreBase Evaluator;
			protected AggregateProcessingParam(ExpressionEvaluatorCoreBase evaluator) {
				this.Evaluator = evaluator;
			}
			public abstract object GetResult();
			public abstract bool Process(object operand);	
		}
		class ExistsProcessingParam : AggregateProcessingParam {
			bool result = false;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				result = true;
				return true;
			}
			public ExistsProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class CountProcessingParam : AggregateProcessingParam {
			int result = 0;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				++result;
				return false;
			}
			public CountProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class MinProcessingParam : AggregateProcessingParam {
			object result = null;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				if(operand != null) {
					if(result == null || Evaluator.Compare(operand, result) < 0) {
						result = operand;
					}
				}
				return false;
			}
			public MinProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class MaxProcessingParam : AggregateProcessingParam {
			object result = null;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				if(operand != null) {
					if(result == null || Evaluator.Compare(operand, result) > 0) {
						result = operand;
					}
				}
				return false;
			}
			public MaxProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class SumProcessingParam : AggregateProcessingParam {
			object result = null;
			public override object GetResult() {
				return result;
			}
			public override bool Process(object operand) {
				if(operand != null) {
					if(result == null) {
						result = operand;
					} else {
						result = Evaluator.Process(new BinaryOperator(new OperandValue(result), new OperandValue(operand), BinaryOperatorType.Plus));
					}
				}
				return false;
			}
			public SumProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		class AvgProcessingParam : AggregateProcessingParam {
			object result = null;
			int count = 0;
			public override object GetResult() {
				if(count == 0)
					return null;
				else
					return Evaluator.Process(new BinaryOperator(new OperandValue(result), new OperandValue(count), BinaryOperatorType.Divide));
			}
			public override bool Process(object operand) {
				if(operand != null) {
					if(result == null) {
						result = operand;
					} else {
						result = Evaluator.Process(new BinaryOperator(new OperandValue(result), new OperandValue(operand), BinaryOperatorType.Plus));
					}
					++count;
				}
				return false;
			}
			public AvgProcessingParam(ExpressionEvaluatorCoreBase evaluator) : base(evaluator) { }
		}
		void DoAggregate(AggregateProcessingParam param, IEnumerable contextsCollection, CriteriaOperator filterExpression, CriteriaOperator expression) {
			if(contextsCollection != null) {
				foreach(EvaluatorContext subContext in contextsCollection) {
					SetContext(subContext);
					if(this.Fit(filterExpression)) {
						object candidate = null;
						if(!ReferenceEquals(expression, null)) {
							candidate = this.Process(expression);
							if(candidate == null)
								continue;
						}
						if(param.Process(candidate))
							return;
					}
				}
			}
		}
		protected object DoAggregate(Aggregate aggregateType, IEnumerable contextsCollection, CriteriaOperator filterExpression, CriteriaOperator expression) {
			AggregateProcessingParam param;
			switch(aggregateType) {
				case Aggregate.Exists:
					param = new ExistsProcessingParam(this);
					break;
				case Aggregate.Count:
					param = new CountProcessingParam(this);
					break;
				case Aggregate.Avg:
					param = new AvgProcessingParam(this);
					break;
				case Aggregate.Max:
					param = new MaxProcessingParam(this);
					break;
				case Aggregate.Min:
					param = new MinProcessingParam(this);
					break;
				case Aggregate.Sum:
					param = new SumProcessingParam(this);
					break;
				default:
					throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotImplemented, typeof(AggregateOperand).Name, aggregateType.ToString()));
			}
			DoAggregate(param, contextsCollection, filterExpression, expression);
			return param.GetResult();
		}
		public object Evaluate(EvaluatorContext evaluationContext, CriteriaOperator evaluatorCriteria) {
			return Evaluate(evaluationContext, evaluatorCriteria, null);
		}
		public object Evaluate(EvaluatorContext evaluationContext, CriteriaOperator evaluatorCriteria, IComparer customComparer) {
			System.Diagnostics.Debug.Assert(!HasContext);
			try {
				this.customComparer = customComparer;
				SetContext(evaluationContext);
				return this.Process(evaluatorCriteria);
			} finally {
				ClearContext();
			}
		}
		protected abstract bool HasContext { get; }
		protected abstract void SetContext(EvaluatorContext context);
		protected abstract void ClearContext();
		protected abstract EvaluatorContext GetContext();
		public bool Fit(EvaluatorContext evaluationContext, CriteriaOperator filterCriteria) {
			if(ReferenceEquals(filterCriteria, null))
				return true;
			return (bool)Evaluate(evaluationContext, filterCriteria);
		}
		protected bool Fit(CriteriaOperator filterCriteria) {
			if(ReferenceEquals(filterCriteria, null))
				return true;
			return (bool)Process(filterCriteria);
		}
	}
	public class EvaluatorCriteriaValidator : IClientCriteriaVisitor {
#if !SL
		readonly PropertyDescriptorCollection Properties;
		public EvaluatorCriteriaValidator(PropertyDescriptorCollection properties) {
			this.Properties = properties;
		}
#endif
		public virtual object Visit(BetweenOperator theOperator) {
			Validate(theOperator.TestExpression);
			Validate(theOperator.EndExpression);
			Validate(theOperator.BeginExpression);
			return null;
		}
		public virtual object Visit(BinaryOperator theOperator) {
			Validate(theOperator.LeftOperand);
			Validate(theOperator.RightOperand);
			return null;
		}
		public virtual object Visit(UnaryOperator theOperator) {
			Validate(theOperator.Operand);
			return null;
		}
		public virtual object Visit(InOperator theOperator) {
			Validate(theOperator.LeftOperand);
			Validate(theOperator.Operands);
			return null;
		}
		public virtual object Visit(GroupOperator theOperator) {
			Validate(theOperator.Operands);
			return null;
		}
		public virtual object Visit(OperandValue theOperand) {
			return null;
		}
		public virtual object Visit(FunctionOperator theOperator) {
			Validate(theOperator.Operands);
			return null;
		}
		public virtual object Visit(OperandProperty theOperand) {
			if(theOperand.PropertyName.IndexOf('.') < 0
#if !SL
				&& Properties.Find(theOperand.PropertyName, false) == null
#endif
				&& !EvaluatorProperty.GetIsThisProperty(theOperand.PropertyName)) {
				throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, theOperand.ToString()));
			}
			return null;
		}
		public virtual object Visit(AggregateOperand theOperand) {
			if(theOperand.IsTopLevel)
				throw new InvalidOperationException("can't evaluate top level aggregate on single objct, collection property expected");	
			Validate(theOperand.CollectionProperty);
			return null;
		}
		public virtual object Visit(JoinOperand theOperand) {
			return null;
		}
		public void Validate(CriteriaOperator criteria) {
			if(!ReferenceEquals(criteria, null))
				criteria.Accept(this);
		}
		public void Validate(IList operands) {
			foreach(CriteriaOperator operand in operands)
				Validate(operand);
		}
	}
	public class ExpressionEvaluator {
		protected virtual ExpressionEvaluatorCoreBase EvaluatorCore { get { return evaluatorCore; } }
		protected readonly EvaluatorContextDescriptor DefaultDescriptor;
		protected readonly CriteriaOperator evaluatorCriteria;
		readonly ExpressionEvaluatorCoreBase evaluatorCore;
		protected ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore) {
			if(doCreateEvaluatorCore) this.evaluatorCore = new ExpressionEvaluatorCore(caseSensitive, new EvaluateCustomFunctionHandler(EvaluateCustomFunction));
			this.DefaultDescriptor = descriptor;
			this.evaluatorCriteria = criteria;
		}
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive) : this(descriptor, criteria, caseSensitive, true) { }
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria) : this(descriptor, criteria, true) { }
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore, ICollection<ICustomFunctionOperator> customFunctions)
			: this(descriptor, criteria, caseSensitive, doCreateEvaluatorCore) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) 
			: this(descriptor, criteria, caseSensitive) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(descriptor, criteria, true) {
			RegisterCustomFunctions(customFunctions);
		}
#if !SL
		public IEvaluatorDataAccess DataAccess { set { ((EvaluatorContextDescriptorDefault)DefaultDescriptor).DataAccess = value; } }
		protected ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvauluatorCore)
			: this(new EvaluatorContextDescriptorDefault(properties), criteria, caseSensitive, doCreateEvauluatorCore) {
			new EvaluatorCriteriaValidator(properties).Validate(this.evaluatorCriteria);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive) : this(properties, criteria, caseSensitive, true) { }
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria) : this(properties, criteria, true) { }
		public ExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, bool caseSensitive) : this(properties, CriteriaOperator.Parse(criteria), caseSensitive) { }
		public ExpressionEvaluator(PropertyDescriptorCollection properties, string criteria) : this(properties, criteria, true) { }
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria, caseSensitive, doCreateEvaluatorCore) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria, caseSensitive) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria, caseSensitive) {
			RegisterCustomFunctions(customFunctions);
		}
		public ExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, ICollection<ICustomFunctionOperator> customFunctions)
			: this(properties, criteria) {
			RegisterCustomFunctions(customFunctions);
		}
#endif
		protected virtual EvaluatorContext PrepareContext(object valuesSource) {
			return new EvaluatorContext(DefaultDescriptor, valuesSource);
		}
		public object Evaluate(object theObject) {
			return Evaluate(theObject, null);
		}
		public object Evaluate(object theObject, IComparer customComparer) {
			return EvaluatorCore.Evaluate(PrepareContext(theObject), evaluatorCriteria, customComparer);
		}
		public bool Fit(object theObject) {
			return EvaluatorCore.Fit(PrepareContext(theObject), evaluatorCriteria);
		}
		bool throwExceptionIfNotFoundCustomFunction = true;
		public bool ThrowExceptionIfNotFoundCustomFunction {
			get { return throwExceptionIfNotFoundCustomFunction; }
			set { throwExceptionIfNotFoundCustomFunction = value; }
		}
		CustomFunctionCollection customFunctionCollection;
		void RegisterCustomFunctions(ICollection<ICustomFunctionOperator> customFunctions) {
			if(customFunctions is CustomFunctionCollection) {
				customFunctionCollection = (CustomFunctionCollection)customFunctions;
				return;
			}
			customFunctionCollection = new CustomFunctionCollection();
			if(customFunctions == null) return;
			foreach(ICustomFunctionOperator customFunction in customFunctions) {
				customFunctionCollection.Add(customFunction);
			}
		}
		protected virtual object EvaluateCustomFunction(string functionName, params object[] operands) {
			if(customFunctionCollection == null && ThrowExceptionIfNotFoundCustomFunction) {
				if(ThrowExceptionIfNotFoundCustomFunction) throw new NotImplementedException();
				else return null;
			}
			ICustomFunctionOperator customFunction = customFunctionCollection.GetCustomFunction(functionName);
			if(customFunction == null) {
				if(ThrowExceptionIfNotFoundCustomFunction) throw new NotImplementedException();
				else return null;
			}
			return customFunction.Evaluate(operands);
		}
	}
	internal class BooleanCriteriaStateObject {
		public static readonly object Logical = BooleanCriteriaState.Logical;
		public static readonly object Value = BooleanCriteriaState.Value;
		public static readonly object Undefined = BooleanCriteriaState.Undefined;
	}
	public enum BooleanCriteriaState {
		Logical,
		Value,
		Undefined
	}
	public class BooleanComplianceChecker: IClientCriteriaVisitor {
		const string MustBeArithmetical = "Must be arithmetical bool";
		const string MustBeLogical = "Must be logical bool";
		static BooleanComplianceChecker instance = new BooleanComplianceChecker();
		public static BooleanComplianceChecker Instance { get { return instance; } }
		public object Visit(AggregateOperand theOperand) {
			if(theOperand.AggregateType == Aggregate.Exists){
				return BooleanCriteriaStateObject.Logical;
			}
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(JoinOperand theOperand) {
			if(theOperand.AggregateType == Aggregate.Exists) {
				return BooleanCriteriaStateObject.Logical;
			}
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(OperandProperty theOperand) {
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(BetweenOperator theOperator) {
			BooleanCriteriaState testExRes = Process(theOperator.TestExpression);
			BooleanCriteriaState beginExRes = Process(theOperator.BeginExpression);
			BooleanCriteriaState endExRes = Process(theOperator.EndExpression);
			if (testExRes == BooleanCriteriaState.Logical 
				|| beginExRes == BooleanCriteriaState.Logical 
				|| endExRes == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			return BooleanCriteriaStateObject.Logical;
		}
		public object Visit(BinaryOperator theOperator) {
			BooleanCriteriaState leftRes = Process(theOperator.LeftOperand);
			BooleanCriteriaState rightRes = Process(theOperator.RightOperand);
			if (leftRes == BooleanCriteriaState.Logical || rightRes == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			switch (theOperator.OperatorType) {
				case BinaryOperatorType.Equal:
				case BinaryOperatorType.Greater:
				case BinaryOperatorType.GreaterOrEqual:
				case BinaryOperatorType.Less:
				case BinaryOperatorType.LessOrEqual:
				case BinaryOperatorType.Like:
				case BinaryOperatorType.NotEqual:
					return BooleanCriteriaStateObject.Logical;
			}
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(UnaryOperator theOperator) {
			BooleanCriteriaState res = Process(theOperator.Operand);
			if (theOperator.OperatorType == UnaryOperatorType.Not) {
				if (res == BooleanCriteriaState.Value) throw new ArgumentException(MustBeLogical);
				return BooleanCriteriaStateObject.Logical;
			}
			if (res == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			if(theOperator.OperatorType == UnaryOperatorType.IsNull) return BooleanCriteriaStateObject.Logical;
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(InOperator theOperator) {
			BooleanCriteriaState leftRes = Process(theOperator.LeftOperand);
			if (leftRes == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			foreach (CriteriaOperator co in theOperator.Operands) {
				BooleanCriteriaState coRes = Process(co);
				if (coRes == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			}
			return BooleanCriteriaStateObject.Logical;
		}
		public object Visit(GroupOperator theOperator) {
			foreach (CriteriaOperator co in theOperator.Operands) {
				BooleanCriteriaState coRes = Process(co);
				if (coRes == BooleanCriteriaState.Value) throw new ArgumentException(MustBeLogical);
			}
			return BooleanCriteriaStateObject.Logical;
		}
		public object Visit(OperandValue theOperand) {
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(FunctionOperator theOperator) {
			if (theOperator.OperatorType == FunctionOperatorType.Iif) {
				if(Process(theOperator.Operands[0]) == BooleanCriteriaState.Value)throw new ArgumentException(MustBeLogical);
				if(Process(theOperator.Operands[1]) == BooleanCriteriaState.Logical)throw new ArgumentException(MustBeArithmetical);
				if(Process(theOperator.Operands[2]) == BooleanCriteriaState.Logical)throw new ArgumentException(MustBeArithmetical);
				return BooleanCriteriaStateObject.Value;
			}else if(theOperator.OperatorType == FunctionOperatorType.IsNull){
				if(Process(theOperator.Operands[0]) == BooleanCriteriaState.Logical)throw new ArgumentException(MustBeArithmetical);
				if (theOperator.Operands.Count == 2) {
					if (Process(theOperator.Operands[1]) == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
					return BooleanCriteriaStateObject.Value;
				}
				return BooleanCriteriaStateObject.Logical;
			}
			foreach (CriteriaOperator co in theOperator.Operands) {
				if (Process(co) == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			}
			switch (theOperator.OperatorType) {
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
				case FunctionOperatorType.IsNullOrEmpty:
					return BooleanCriteriaStateObject.Logical;
				case FunctionOperatorType.Custom:
					return BooleanCriteriaStateObject.Undefined;
			}
			return BooleanCriteriaStateObject.Value;
		}
		public BooleanCriteriaState Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null)) return BooleanCriteriaState.Logical;
			return (BooleanCriteriaState)operand.Accept(this);
		}
		public void Process(CriteriaOperator operand, bool mustBeLogical) {
			if (ReferenceEquals(operand, null)) return;
			if (mustBeLogical) {
				if ((BooleanCriteriaState)operand.Accept(this) == BooleanCriteriaState.Value) throw new ArgumentException(MustBeLogical);
			} else {
				if ((BooleanCriteriaState)operand.Accept(this) == BooleanCriteriaState.Logical) throw new ArgumentException(MustBeArithmetical);
			}
		}
	}
	public class IsLogicalCriteriaChecker : IClientCriteriaVisitor {
		static IsLogicalCriteriaChecker instance = new IsLogicalCriteriaChecker();
		public static IsLogicalCriteriaChecker Instance { get { return instance; } }
		public static BooleanCriteriaState GetBooleanState(CriteriaOperator operand) {
			return Instance.Process(operand);
		}
		public object Visit(AggregateOperand theOperand) {
			if (theOperand.AggregateType == Aggregate.Exists) {
				return BooleanCriteriaStateObject.Logical;
			}
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(JoinOperand theOperand) {
			if(theOperand.AggregateType == Aggregate.Exists) {
				return BooleanCriteriaStateObject.Logical;
			}
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(OperandProperty theOperand) {
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(BetweenOperator theOperator) {
			return BooleanCriteriaStateObject.Logical;
		}
		public object Visit(BinaryOperator theOperator) {
			switch (theOperator.OperatorType) {
				case BinaryOperatorType.Equal:
				case BinaryOperatorType.Greater:
				case BinaryOperatorType.GreaterOrEqual:
				case BinaryOperatorType.Less:
				case BinaryOperatorType.LessOrEqual:
				case BinaryOperatorType.Like:
				case BinaryOperatorType.NotEqual:
					return BooleanCriteriaStateObject.Logical;
			}
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(UnaryOperator theOperator) {
			if (theOperator.OperatorType == UnaryOperatorType.Not) {
				return BooleanCriteriaStateObject.Logical;
			}
			if(theOperator.OperatorType == UnaryOperatorType.IsNull) return BooleanCriteriaStateObject.Logical;
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(InOperator theOperator) {
			return BooleanCriteriaStateObject.Logical;
		}
		public object Visit(GroupOperator theOperator) {
			return BooleanCriteriaStateObject.Logical;
		}
		public object Visit(OperandValue theOperand) {
			return BooleanCriteriaStateObject.Value;
		}
		public object Visit(FunctionOperator theOperator) {
			if (theOperator.OperatorType == FunctionOperatorType.IsNull) {
				if (theOperator.Operands.Count == 2) {
					return BooleanCriteriaStateObject.Value;
				}
				return BooleanCriteriaStateObject.Logical;
			}
			switch (theOperator.OperatorType) {
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
				case FunctionOperatorType.IsNullOrEmpty:
					return BooleanCriteriaStateObject.Logical;
				case FunctionOperatorType.Custom:
					return BooleanCriteriaStateObject.Undefined;
			}
			return BooleanCriteriaStateObject.Value;
		}
		public BooleanCriteriaState Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null)) return BooleanCriteriaState.Logical;
			return (BooleanCriteriaState)operand.Accept(this);
		}
	}
}
