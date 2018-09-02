using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vts.Test
{
    // Object Dumper(https://gist.github.com/DTTerastar/6642655)
    public class Dumper
    {
        private readonly object _a;
        private readonly bool _breakCircularRefs;
        private readonly IList<object> _l = new List<object>();
        private readonly int _maxDepth;
        private readonly IndentedStringBuilder _sb = new IndentedStringBuilder();
        private int _depth;

        public Dumper(Object a, int maxDepth, bool breakCircularRefs)
        {
            _a = a;
            _maxDepth = maxDepth;
            _breakCircularRefs = breakCircularRefs;
        }

        public static string ToGenericTypeString(Type t)
        {
            if (t.Name.Contains("__AnonymousType")) return t.IsArray ? "[]" : "";
            if (!t.IsGenericType)
                return t.Name;
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            int indexOf = genericTypeName.IndexOf('`');
            if (indexOf > 0)
                genericTypeName = genericTypeName.Substring(0, indexOf);
            string genericArgs = string.Join(",", t.GetGenericArguments().Select(ToGenericTypeString).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        private void Dump(Object o)
        {
            if (o == null)
                _sb.Append("null");
            else
            {
                Type type = o.GetType();
                Type genType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (type.IsEnum) _sb.AppendFormat("{0}.{1}", type.Name, Enum.GetName(type, o));
                else if (type == typeof(string))
                {
                    string s = o.ToString();
                    if (!s.Any(b => b == '\r' || b == '\n' || b == '"' || b == '\\'))
                        _sb.AppendFormat("\"{0}\"", s);
                    else
                        _sb.AppendFormat(@"@""{0}""", s.Replace(@"""", @""""""));
                }
                else if (type == typeof(DataTable))
                {
                    var dt = (DataTable)o;
                    _sb.AppendFormat(string.IsNullOrWhiteSpace(dt.TableName) ? "new DataTable\r\n{{" : "new DataTable(\"{0}\")\r\n{{", dt.TableName);
                    using (_sb.IncreaseIndent())
                    {
                        _sb.Append("\r\nnew Rows = ");
                        foreach (DataRow dr in dt.Rows)
                        {
                            _sb.AppendLine("{");
                            using (_sb.IncreaseIndent())
                            {
                                foreach (DataColumn column in dt.Columns)
                                {
                                    _sb.Append("{");
                                    Dump(column.ColumnName);
                                    _sb.Append(", ");
                                    Dump(dr[column.ColumnName]);
                                    _sb.AppendLine("},");
                                }
                            }
                            _sb.AppendLine("},");
                        }
                    }
                    _sb.Append("}");
                }
                else if (type == typeof(bool)) _sb.Append((bool)o ? "true" : "false");
                else if (type == typeof(DBNull)) _sb.Append("DBNull.Value");
                else if (type == typeof(Guid)) _sb.AppendFormat(@"new Guid(""{0}"")", o);
                else if (type == typeof(DateTime)) _sb.AppendFormat(@"DateTime.ParseExact(""{0:r}"", ""r"")", o);
                else if (genType == typeof(KeyValuePair<,>))
                {
                    _sb.Append("{");
                    Dump(type.GetProperty("Key").GetValue(o, null));
                    _sb.Append(", ");
                    Dump(type.GetProperty("Value").GetValue(o, null));
                    _sb.Append("}");
                }
                else if (type.IsValueType) _sb.Append(o.ToString());
                else if (type.IsSubclassOf(typeof(Type))) _sb.Append(o.ToString());
                else
                {
                    _depth++;
                    try
                    {
                        if (_depth > _maxDepth)
                        {
                            _sb.Append("#depthExceeded");
                            return;
                        }

                        if (_breakCircularRefs)
                        {
                            if (_l.Contains(o)) // This breaks circular references
                            {
                                _sb.Append("#Ref");
                                return;
                            }
                            _l.Add(o);
                        }

                        string genericTypeString = ToGenericTypeString(type);
                        _sb.AppendFormat(string.IsNullOrEmpty(genericTypeString) ? "new {0}{{" : "new {0} {{", genericTypeString);

                        using (_sb.IncreaseIndent())
                        {
                            var dictionary = o as IDictionary;
                            if (dictionary != null)
                            {
                                int i = 0;
                                foreach (DictionaryEntry b in dictionary)
                                    using (_sb.IncreaseIndent())
                                    {
                                        if (i == 0) _sb.AppendLine();
                                        if (i++ > 0) _sb.Append(",\r\n");
                                        _sb.Append("{");
                                        Dump(b.Key);
                                        _sb.Append(", ");
                                        Dump(b.Value);
                                        _sb.Append("}");
                                    }
                                if (i != 0) _sb.AppendLine();
                            }
                            else
                            {
                                var enumerable = o as IEnumerable;
                                if (enumerable != null)
                                {
                                    int i = 0;
                                    foreach (object b in enumerable)
                                    {
                                        if (i == 0) _sb.AppendLine();
                                        if (i++ > 0) _sb.Append(",\r\n");
                                        Dump(b);
                                    }
                                    if (i != 0) _sb.AppendLine();
                                }
                                else
                                {
                                    int i = 0;
                                    foreach (PropertyInfo info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                                    {
                                        if (info.GetIndexParameters().Length != 0) continue;
                                        if (info.Name != "SyncRoot" && info.Name != "ExtensionData")
                                        {
                                            if (i == 0) _sb.AppendLine();

                                            _sb.AppendFormat(i++ == 0 ? "{0} = " : ",\r\n{0} = ", info.Name);
                                            object value = GetValue(o, info);

                                            if (value == o)
                                                _sb.Append("this");
                                            else
                                                Dump(value);
                                        }
                                    }
                                    if (i != 0) _sb.AppendLine();
                                }
                            }
                        }
                        _sb.Append("}");
                    }
                    finally
                    {
                        _depth--;
                    }
                }
            }
        }

        private static object GetValue(object a, PropertyInfo info)
        {
            try
            {
                return info.GetValue(a, null);
            }
            catch (Exception)
            {
                return "#Err";
            }
        }

        public override string ToString()
        {
            if (_sb.Length == 0) Dump(_a);
            return _sb.ToString();
        }
    }
    public class IndentedStringBuilder
    {
        private const int SpacesPerIndent = 2;
        private readonly StringBuilder _sb;
        private string _completeIndentationString = "";
        private int _indent;
        private bool _newline;

        public IndentedStringBuilder()
        {
            _sb = new StringBuilder();
        }

        public int Length
        {
            get { return _sb.Length; }
        }

        public void Append(string value)
        {
            int i = value.IndexOf("\r\n", StringComparison.Ordinal);
            if (i < 0) // No newline
                InternalAppend(value, false);
            else if (i == value.Length - 2) // Ends with newline
                InternalAppend(value, true);
            else
            {
                InternalAppend(value.Substring(0, i + 2), true);
                Append(value.Substring(i + 2));
            }
        }

        private void InternalAppend(string value, bool endsInCr)
        {
            if (_newline)
                _sb.Append(_completeIndentationString);
            _sb.Append(value);
            _newline = endsInCr;
        }

        public void AppendLine()
        {
            Append(Environment.NewLine);
        }

        public void AppendLine(string value)
        {
            Append(value);
            AppendLine();
        }

        public void AppendFormat(string format, params object[] objects)
        {
            Append(string.Format(format, objects));
        }

        public DecreaseIndentOnDispose IncreaseIndent()
        {
            _indent++;
            _completeIndentationString = new string(' ', SpacesPerIndent * _indent);
            return new DecreaseIndentOnDispose(this);
        }

        public void DecreaseIndent()
        {
            if (_indent <= 0) return;
            _indent--;
            _completeIndentationString = new string(' ', SpacesPerIndent * _indent);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
    public class DecreaseIndentOnDispose : IDisposable
    {
        private readonly IndentedStringBuilder _indentedStringBuilder;

        public DecreaseIndentOnDispose(IndentedStringBuilder indentedStringBuilder)
        {
            _indentedStringBuilder = indentedStringBuilder;
        }

        public void Dispose()
        {
            _indentedStringBuilder.DecreaseIndent();
        }
    }
}