using System;
using System.Collections.Generic;
using System.Linq;

namespace Cqrs.Common.Data
{
    public class FilterValueItem
    // When using it in WebApi, pass the class, not the interface because System.Text.Json doesn't support polymorphic serialization yet
    {

        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string Key { get; set; }
        public bool? IsGroup { get; set; }
        /// <summary>
        /// Group only.
        /// </summary>
        public BinaryOperator? BinaryOperator { get; set; }
        /// <summary>
        /// Group only.
        /// </summary>
        public ICollection<FilterValueItem> Items { get; set; } = new List<FilterValueItem>();
        /*
        public class FakeItem
        {
            public string ToSqlString(bool useParameter = false) { return ""; }
            public string ToLambdaString(string paramName = "p", bool isRoot = false) { return ""; }
            public IDictionary<string, object> GetSqlParameters() { return new Dictionary<string, object>(); }
        }
        public List<FakeItem> Items { get; set; } = new List<FakeItem>();
        */

        // public FilterField Field { get; set;  }
        public ComparisonOperator? ComparisonOperator { get; set; }
        public bool? Negate { get; set; }
        public object Value1 { get; set; }
        public object Value2 { get; set; } // for ComparisonOperator.Between

        public string ToJsonString()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return json;
        }

        public string ToSqlString(bool useParameter = false)
        {
            if (IsGroup.HasValue && IsGroup.Value)
            {
                return ToSqlStringGroup(useParameter);
            }
            else
            {
                return ToSqlStringItem(useParameter);
            }
        }

        public string ToLambdaString(string paramName = "p", bool isRoot = false)
        {
            if (IsGroup.HasValue && IsGroup.Value)
            {
                return ToLambdaStringGroup(paramName, isRoot);
            }
            else
            {
                return ToLambdaStringItem(paramName, false);
            }
        }

        public string ToBsonString()
        {
            var result = "";
            if (IsGroup.HasValue && IsGroup.Value)
            {
                result = $@"{{
{ToBsonStringGroup()}
}}";
            }
            else
            {
                result = ToBsonStringItem();
            }
            return result;
        }

        //public Func<T, bool> ToLambdaFunc<T>(string paramName = "p")
        //{
        //    var s = ToLambdaString(paramName, true);
        //    var expr = CSharpScript.EvaluateAsync<Func<T, bool>>(s,
        //        ScriptOptions.Default.WithReferences(typeof(string).Assembly)
        //            .WithReferences(typeof(T).Assembly)
        //            .WithImports("System")).GetAwaiter().GetResult();
        //    return expr;
        //}

        //public async Task<Func<T, bool>> ToLambdaFuncAsync<T>(string paramName = "p")
        //{
        //    var s = ToLambdaString(paramName, true);
        //    var expr = await CSharpScript.EvaluateAsync<Func<T, bool>>(s,
        //        ScriptOptions.Default.WithReferences(typeof(string).Assembly)
        //            .WithReferences(typeof(T).Assembly)
        //            .WithImports("System"));
        //    return expr;
        //}

        public IDictionary<string, object> GetSqlParameters()
        {
            if (IsGroup.HasValue && IsGroup.Value)
            {
                return GetSqlParametersGroup();
            }
            else
            {
                return GetSqlParametersItem();
            }
        }

        private string ToSqlStringItem(bool useParameter = false)
        {
            if (string.IsNullOrEmpty(FieldName)) return string.Empty;
            string target = string.Empty;
            // use field name for parameters, to avoid column with same name, but different sources
            if (Value1 != null)
                target = (useParameter ? "@" + FieldName : QuoteValue(Value1.ToString(), Value1.GetType().FullName, "'"));
            else
                target = "@" + FieldName;
            var compOp = ComparisonOperator.HasValue ? ComparisonOperator.Value : Data.ComparisonOperator.Equal;
            switch (compOp)
            {
                case Data.ComparisonOperator.Contains:
                    target = "'%' + " + target + " + '%'";
                    break;
                case Data.ComparisonOperator.StartsWith:
                    target = target + " + '%'";
                    break;
                case Data.ComparisonOperator.EndsWith:
                    target = "'%' + " + target;
                    break;
                case Data.ComparisonOperator.Between:
                    if (Value2 != null)
                        target = $"{target} AND " + (useParameter ? "@" + $"{FieldName}2" : QuoteValue(Value2.ToString(), Value2.GetType().FullName, "'"));
                    else
                        target = $"{target} AND @{FieldName}2";
                    break;
                default:
                    break;
            }
            var negate = Negate.HasValue && Negate.Value;
            var sqlString = (negate ? "NOT(" : "") + $"[{FieldName}]" + " " + GetComparisonOperatorSql(compOp) + " " + target + (negate ? ")" : "");
            return sqlString;
        }
        private string ToSqlStringGroup(bool useParameter = false)
        {
            if (!Items.Any()) return string.Empty;
            var sqlString = "(";
            foreach (var item in Items)
            {
                if (sqlString == "(")
                {
                    sqlString += $"{item.ToSqlString(useParameter) }";
                }
                else
                {
                    var binOp = BinaryOperator.HasValue ? BinaryOperator.Value : Data.BinaryOperator.And;
                    string sOp = binOp == Data.BinaryOperator.Or ? "OR" : "AND";
                    sqlString = $"{sqlString} {sOp} {item.ToSqlString(useParameter) }";
                }
            }
            sqlString += ")";
            return sqlString;
        }

        private string ToLambdaStringItem(string paramName = "p", bool isRoot = false)
        {
            string target = string.Empty;
            target = QuoteValue(Value1?.ToString(), Value1?.GetType().FullName, "\"");
            switch (FieldType)
            {
                case "DateTime":
                    target = $"DateTime.Parse({target})";
                    break;
            }
            var sqlString = string.Empty;
            var compOp = ComparisonOperator.HasValue ? ComparisonOperator.Value : Data.ComparisonOperator.Equal;
            var negate = Negate.HasValue && Negate.Value;
            switch (compOp)
            {
                case Data.ComparisonOperator.Contains:
                    target = ".Contains(" + target + ")";
                    sqlString = (negate ? "!(" : "") + $"{paramName}.{FieldName}{target}" + (negate ? ")" : "");
                    break;
                case Data.ComparisonOperator.StartsWith:
                    target = ".StartsWith(" + target + ")";
                    sqlString = (negate ? "!(" : "") + $"{paramName}.{FieldName}{target}" + (negate ? ")" : "");
                    break;
                case Data.ComparisonOperator.EndsWith:
                    target = ".EndsWith(" + target + ")";
                    sqlString = (negate ? "!(" : "") + $"{paramName}.{FieldName}{target}" + (negate ? ")" : "");
                    break;
                case Data.ComparisonOperator.Between:
                    var target2 = QuoteValue(Value2?.ToString(), Value2?.GetType().FullName, "\"");
                    switch (FieldType)
                    {
                        case "DateTime":
                            target2 = $"DateTime.Parse({target2})";
                            break;
                    }
                    var part1 = $"{paramName}.{FieldName} >= {target}";
                    var part2 = $"{paramName}.{FieldName} <= {target2}";
                    sqlString = (negate ? "!(" : "") + $"{part1} && {part2}" + (negate ? ")" : "");
                    break;
                default:
                    sqlString = (negate ? "!(" : "") + $"{paramName}.{FieldName} {GetComparisonOperatorCSharp(compOp)} {target}" + (negate ? ")" : "");
                    break;
            }
            return sqlString;
        }
        private string ToLambdaStringGroup(string paramName = "p", bool isRoot = false)
        {
            if (!Items.Any()) return string.Empty;
            var sqlString = "(";
            foreach (var item in Items)
            {
                if (sqlString == "(")
                {
                    sqlString += $"{item.ToLambdaString(paramName, false) }";
                }
                else
                {
                    var binOp = BinaryOperator.HasValue ? BinaryOperator.Value : Data.BinaryOperator.And;
                    string sOp = binOp == Data.BinaryOperator.Or ? "||" : "&&";
                    sqlString = $"{sqlString} {sOp} {item.ToLambdaString(paramName, false) }";
                }
            }
            sqlString += ")";
            return isRoot ? $"{paramName} => {sqlString}" : sqlString;
        }

        private string ToBsonStringItem()
        {
            if (string.IsNullOrEmpty(FieldName)) return string.Empty;
            string target = string.Empty;
            // use field name for parameters, to avoid column with same name, but different sources
            var compOp = ComparisonOperator.HasValue ? ComparisonOperator.Value : Data.ComparisonOperator.Equal;
            var negate = Negate.HasValue && Negate.Value;

            switch (compOp)
            {
                case Data.ComparisonOperator.Contains:
                    target = "\"" + Value1?.ToString() + "\"";
                    break;
                case Data.ComparisonOperator.StartsWith:
                    target = "\"^" + Value1?.ToString() + "\"";
                    break;
                case Data.ComparisonOperator.EndsWith:
                    target = "\"" + Value1?.ToString() + "$\"";
                    break;
                case Data.ComparisonOperator.Between:
                    // TODO
                    break;
                default:
                    if (Value1 != null)
                        target = (QuoteValueBson(Value1, FieldName, FieldType, "\""));
                    else
                        target = (QuoteValueBson(FieldName, FieldName, FieldType, "\"")); ;
                    break;
            }
            // var bsonString = (Negate ? "NOT(" : "") + $"[{FieldName}]" + " " + GetComparisonOperatorSql(ComparisonOperator) + " " + target + (Negate ? ")" : "");
            var bsonString = "";
            if (negate)
            {
                switch (compOp)
                {
                    case Data.ComparisonOperator.Equal:
                        bsonString = @$"{{ ""{FieldName}"": {{ ""$ne"": {target} }} }}";
                        break;
                    case Data.ComparisonOperator.Between:
                        bsonString = @$"
{{ 
    ""$and"": [
{{ ""{FieldName}"": {{ ""$lt"": {QuoteValueBson(Value1, FieldName, FieldType, "\"")} }} }},
{{ ""{FieldName}"": {{ ""$gt"": {QuoteValueBson(Value2, FieldName, FieldType, "\"")} }} }},
]
}}";
                        break;
                    default:
                        var sCompOp = GetComparisonOperatorBson(compOp);
                        var sOptions = sCompOp == "$regex" ? ", \"$options\": \"i\"" : "";
                        bsonString = @$"{{ ""{FieldName}"": {{ ""$not"": {{ ""{GetComparisonOperatorBson(compOp)}"": {target} {sOptions} }} }} }}";
                        break;
                }
            }
            else
            {
                switch (compOp)
                {
                    case Data.ComparisonOperator.Equal:
                        bsonString = @$"{{ ""{FieldName}"": {target} }}";
                        break;
                    case Data.ComparisonOperator.Between:
                        bsonString = @$"
{{ 
    ""$and"": [
{{ ""{FieldName}"": {{ ""$gte"": {QuoteValueBson(Value1, FieldName, FieldType, "\"")} }} }},
{{ ""{FieldName}"": {{ ""$lte"": {QuoteValueBson(Value2, FieldName, FieldType, "\"")} }} }},
]
}}";
                        break;
                    default:
                        var sCompOp = GetComparisonOperatorBson(compOp);
                        var sOptions = sCompOp == "$regex" ? ", \"$options\": \"i\"" : "";
                        bsonString = @$"{{ ""{FieldName}"": {{ ""{sCompOp}"": {target} { sOptions } }} }}";
                        break;
                }
            }
            return bsonString;
        }
        private string ToBsonStringGroup()
        {
            if (!Items.Any()) return string.Empty;
            var bsonString = "";
            var isEmpty = true;
            var binOp = BinaryOperator.HasValue ? BinaryOperator.Value : Data.BinaryOperator.And;
            //if (BinaryOperator == BinaryOperator.And)
            //{
            //    bsonString = $@"";
            //}
            //else
            //{
            //}
            if (binOp == Data.BinaryOperator.And)
            {
                bsonString = $@"
""$and"": [
";
            }
            else
            {
                bsonString = $@"
""$or"": [
";
            }
            foreach (var item in Items)
            {
                if (isEmpty)
                {
                    bsonString += $"{item.ToBsonString()}";
                    isEmpty = false;
                }
                else
                {
                    bsonString = $@"
{bsonString},
{item.ToBsonString()}
";
                }
            }
            bsonString += "]";
            return bsonString;
        }

        private IDictionary<string, object> GetSqlParametersItem()
        {
            var dict = new Dictionary<string, object>()
            {
                { $"@{FieldName}", Value1 }
            };
            var compOp = ComparisonOperator.HasValue ? ComparisonOperator.Value : Data.ComparisonOperator.Equal;
            var negate = Negate.HasValue && Negate.Value;
            if (compOp == Data.ComparisonOperator.Between)
            {
                dict.Add($"@{FieldName}2", Value2);
            }
            return dict;
        }

        private IDictionary<string, object> GetSqlParametersGroup()
        {
            var dict = new Dictionary<string, object>();
            foreach (var item in Items)
            {
                foreach (var p in item.GetSqlParameters())
                {
                    if (!dict.ContainsKey(p.Key))
                    {
                        dict.Add(p.Key, p.Value);
                    }
                }
            }
            return dict;
        }

        public static string GetComparisonOperatorCSharp(ComparisonOperator op)
        {
            string eOp = string.Empty;
            switch (op)
            {
                case Data.ComparisonOperator.Equal:
                    eOp = "==";
                    break;
                case Data.ComparisonOperator.NotEqual:
                    eOp = "!=";
                    break;
                case Data.ComparisonOperator.LessThan:
                    eOp = "<";
                    break;
                case Data.ComparisonOperator.LessOrEqualThan:
                    eOp = "<=";
                    break;
                case Data.ComparisonOperator.GreaterThan:
                    eOp = ">";
                    break;
                case Data.ComparisonOperator.GreaterOrEqualThan:
                    eOp = ">=";
                    break;
                case Data.ComparisonOperator.IsNull:
                    eOp = "== null";
                    break;
                case Data.ComparisonOperator.IsNotNull:
                    eOp = "!= null";
                    break;
                case Data.ComparisonOperator.Contains:
                    eOp = "LIKE"; // TODO: implement
                    break;
                default:
                    break;
            }
            return eOp;
        }

        public static string GetComparisonOperatorSql(ComparisonOperator op)
        {
            string eOp = string.Empty;
            switch (op)
            {
                case Data.ComparisonOperator.Equal:
                    eOp = "=";
                    break;
                case Data.ComparisonOperator.NotEqual:
                    eOp = "<>";
                    break;
                case Data.ComparisonOperator.LessThan:
                    eOp = "<";
                    break;
                case Data.ComparisonOperator.LessOrEqualThan:
                    eOp = "<=";
                    break;
                case Data.ComparisonOperator.GreaterThan:
                    eOp = ">";
                    break;
                case Data.ComparisonOperator.GreaterOrEqualThan:
                    eOp = ">=";
                    break;
                case Data.ComparisonOperator.IsNull:
                    eOp = "IS NULL";
                    break;
                case Data.ComparisonOperator.Contains:
                case Data.ComparisonOperator.StartsWith:
                case Data.ComparisonOperator.EndsWith:
                    eOp = "LIKE";
                    break;
                case Data.ComparisonOperator.Between:
                    eOp = "BETWEEN";
                    break;
                default:
                    break;
            }
            return eOp;
        }
        public static string GetComparisonOperatorBson(ComparisonOperator op)
        {
            string eOp = string.Empty;
            switch (op)
            {
                case Data.ComparisonOperator.Equal:
                    eOp = "";
                    break;
                case Data.ComparisonOperator.NotEqual:
                    eOp = "$ne";
                    break;
                case Data.ComparisonOperator.LessThan:
                    eOp = "$lt";
                    break;
                case Data.ComparisonOperator.LessOrEqualThan:
                    eOp = "$lte";
                    break;
                case Data.ComparisonOperator.GreaterThan:
                    eOp = "$gt";
                    break;
                case Data.ComparisonOperator.GreaterOrEqualThan:
                    eOp = "$gte";
                    break;
                case Data.ComparisonOperator.IsNull:
                    eOp = ""; // TODO
                    break;
                case Data.ComparisonOperator.Contains:
                case Data.ComparisonOperator.StartsWith:
                case Data.ComparisonOperator.EndsWith:
                    eOp = "$regex"; // Regex pattern
                    break;
                case Data.ComparisonOperator.Between:
                    eOp = ""; // composite
                    break;
                default:
                    break;
            }
            return eOp;
        }
        public static string QuoteValue(string value, string typeName, string quote)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(typeName)) return string.Empty;
            string qValue = value;
            var t = typeName;
            if (t == null) t = string.Empty;
            switch (t.ToUpper())
            {
                case "":
                    qValue = quote + qValue + quote;
                    break;
                case "STRING":
                    qValue = quote + qValue + quote;
                    break;
                case "DATETIME":
                    qValue = quote + qValue + quote;
                    break;
                default:
                    break;
            }
            return qValue;
        }
        public static string QuoteValueBson(object value, string fieldName, string typeName, string quote)
        {
            string qValue = value?.ToString();
            if (fieldName == "_id")
            {
                qValue = "ObjectId(" + quote + qValue + quote + ")";
                return qValue;
            }
            var t = typeName;
            if (t == null) t = string.Empty;
            switch (t.ToUpper())
            {
                case "":
                    qValue = quote + qValue + quote;
                    break;
                case "STRING":
                    qValue = quote + qValue + quote;
                    break;
                case "DATETIME":
                    qValue = "ISODate(" + quote + Convert.ToDateTime(value).ToString("s") + quote + ")";
                    break;
                case "BOOLEAN":
                    qValue = qValue.ToLower();
                    break;
                default:
                    break;
            }
            return qValue;
        }

    }
}
