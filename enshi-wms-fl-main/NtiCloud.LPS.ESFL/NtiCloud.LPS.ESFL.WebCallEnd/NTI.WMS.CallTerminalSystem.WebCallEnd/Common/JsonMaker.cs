using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd
{
    public class JsonValue
    {
        private object _value;
        private bool _isFunction;

        protected JsonValue()
        {
        }
        private JsonValue(object value, bool isFunction = false)
        {
            _value = value;
            _isFunction = isFunction;
        }

        protected void SetValue(object value)
        {
            _value = value;
        }
        internal object GetValue()
        {

            if (new Type[] { typeof(JsonObject), typeof(JsonArray) }.Contains(this.GetType()))
            {
                return this;
            }
            else
            {
                return _value;
            }


        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_value == null || _value == DBNull.Value)
            {
                sb.Append("null");
            }
            else
            {
                switch (_value.GetType().Name)
                {
                    case "String":
                        if (_isFunction)
                            sb.Append(_value.ToString());
                        else
                            sb.Append("\"" + SpecialChar(_value.ToString()) + "\"");
                        break;
                    case "Boolean":
                        sb.Append(_value.ToString().ToLower());
                        break;
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "IntPtr":
                    case "UInt16":
                    case "UInt32":
                    case "UInt64":
                    case "UIntPtr":
                    case "Decimal":
                    case "Single":
                    case "Double":
                    case "SByte":
                    case "Byte":
                        sb.Append(_value.ToString());
                        break;
                    default:
                        sb.Append("\"" + SpecialChar(_value.ToString()) + "\"");
                        break;
                }
            }
            return sb.ToString();
        }
        private string SpecialChar(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            value = value.Replace("\\", "\\\\");//对斜线的转义  
            value = value.Replace("\n", "\\n");
            value = value.Replace("\r", "\\r");
            value = value.Replace("\t", "\\t");
            value = value.Replace("\b", "\\b");
            value = value.Replace("\f", "\\f");
            value = value.Replace("\"", "\\\"");
            return value;
        }
        protected JsonValue MakeValue(object value, bool isFunction)
        {

            if (value != null && new Type[] { typeof(JsonValue), typeof(JsonObject), typeof(JsonArray) }.Contains(value.GetType()))
            {
                return value as JsonValue;
            }
            else
            {
                return new JsonValue(value, isFunction);
            }
        }
        protected virtual int MakeJson(string json)
        {
            return 0;
        }



        protected void skipChar(string json, ref int index)
        {
            for (; index < json.Length; index++)
            {
                bool bComment = false;
                bool bStartComment = false;
                bool bLongComment = false;
                switch (json.Substring(index, 1))
                {
                    case "\r":
                    case "\n":
                        if (!bLongComment)
                        {
                            bStartComment = false;
                            bComment = false;
                        }
                        break;
                    case "\t":
                    case " ":
                    case "(":
                    case ")":
                        break;
                    case "//":
                        if (!bLongComment)
                        {
                            if (bStartComment)
                            {
                                bStartComment = false;
                                bComment = true;
                            }
                            else
                            {
                                bStartComment = true;
                                bComment = false;
                                bLongComment = false;
                            }
                        }
                        else
                        {
                            if (bStartComment)
                            {
                                bLongComment = false;
                                bStartComment = false;
                                bComment = false;
                            }
                        }
                        break;
                    case "*":
                        if (bStartComment)
                        {
                            bStartComment = false;
                            bComment = true;
                            bLongComment = true;
                        }
                        else
                        {
                            bStartComment = true;
                        }
                        break;
                    default:
                        if (!bComment)
                        {
                            return;
                        }
                        break;
                }
            }
        }
        protected string parseKey(string json, ref int index)
        {
            bool dquote = false;
            bool squote = false;
            string result = "";
            skipChar(json, ref index);
            for (; index < json.Length; index++)
            {
                string sChar = json.Substring(index, 1);
                switch (sChar)
                {
                    case "\"":
                        dquote = !dquote;
                        if (!dquote)
                        {
                            index++;
                            skipChar(json, ref index);
                            if (json.Substring(index, 1) != ":")
                            {
                                throw new Exception("无效的Key：" + result + ",位置：" + index.ToString());
                            }
                            else
                            {
                                index++;
                                return result;
                            }
                        }
                        break;
                    case "'":
                        squote = !squote;
                        if (!squote)
                        {
                            index++;
                            skipChar(json, ref index);
                            if (json.Substring(index, 1) != ":")
                            {
                                throw new Exception("无效的Key：" + result + ",位置：" + index.ToString());
                            }
                            else
                            {
                                index++;
                                return result;
                            }
                        }
                        break;
                    default:
                        if (!"\r\n\t ".Contains(sChar))
                        {
                            result += sChar;
                        }
                        break;
                }
            }
            return result;
        }
        protected JsonValue MakeValue(string json, ref int index)
        {
            JsonValue result;
            skipChar(json, ref index);
            string sChar = json.Substring(index, 1);
            switch (sChar)
            {
                case "{":
                    result = new JsonObject();
                    index += result.MakeJson(json.Substring(index));
                    return result;
                case "[":
                    result = new JsonArray();
                    index += result.MakeJson(json.Substring(index));
                    return result;
                case "\"":
                case "'":
                    return parseString(json, ref index);
                case "t":
                case "f":
                    return parseBoolean(json, ref index);
                case "n":
                    return parseNull(json, ref index);
                default:
                    return parseNumber(json, ref index);
            }

        }

        protected JsonValue parseString(string json, ref int index)
        {

            StringBuilder sb = new StringBuilder();
            string sChar = json.Substring(index, 1);
            bool bsquote = sChar == "'";
            bool bdquote = sChar == "\"";

            for (index++; index < json.Length; index++)
            {
                sChar = json.Substring(index, 1);
                switch (sChar)
                {
                    case "\\":
                        sChar = json.Substring(++index, 1);
                        switch (sChar)
                        {
                            case "\"":
                            case "\\":
                            case "/":
                            case "'":
                                sb.Append(sChar);
                                break;
                            case "b":
                                sb.Append("\b");
                                break;
                            case "f":
                                sb.Append("\f");
                                break;
                            case "n":
                                sb.Append("\n");
                                break;
                            case "r":
                                sb.Append("\r");
                                break;
                            case "t":
                                sb.Append("\t");
                                break;
                            case "u":
                                sb.Append((char)int.Parse(json.Substring(++index, 4), System.Globalization.NumberStyles.HexNumber));
                                index += 3;
                                break;
                        }
                        break;
                    case "\"":
                        if (bdquote)
                        {
                            index++;
                            return new JsonValue(sb.ToString());
                        }
                        else
                        {
                            sb.Append(sChar);
                        }
                        break;
                    case "'":
                        if (bsquote)
                        {
                            index++;
                            return new JsonValue(sb.ToString());
                        }
                        else
                        {
                            sb.Append(sChar);
                        }
                        break;
                    default:
                        sb.Append(sChar);
                        break;
                }
            }
            throw new Exception("Json对象缺少" + (bsquote ? "\"'\":" : "'\"':") + json.Substring(json.Length - 20));
        }

        protected JsonValue parseBoolean(string json, ref int index)
        {
            if (json.Substring(index, 4) == "true")
            {
                index += 4;
                return new JsonValue(true);
            }
            else if (json.Substring(index, 5) == "false")
            {
                index += 5;
                return new JsonValue(false);
            }
            else
            {
                throw new Exception("无效的Boolean，位置" + index + "，" + json.Substring(index, 20));
            }
        }
        protected JsonValue parseNull(string json, ref int index)
        {
            if (json.Substring(index, 4) == "null")
            {
                index += 4;
                return new JsonValue(null);
            }
            else
            {
                throw new Exception("无效的null，位置" + index + "，" + json.Substring(index, 20));
            }
        }

        protected JsonValue parseNumber(string json, ref int index)
        {
            string value = "";
            for (; index < json.Length; index++)
            {
                string sChar = json.Substring(index, 1);
                if ("+-0123456789.eE".Contains(sChar))
                {
                    value += sChar;
                }
                else
                {
                    break;
                }
            }
            return new JsonValue(decimal.Parse(value));
        }

    }
    public class JsonObject : JsonValue
    {
        private Dictionary<string, JsonValue> _dicData = new Dictionary<string, JsonValue>();
        public JsonObject() { }
        public JsonObject(string json)
        {
            Parse(json);
        }

        public JsonObject(Dictionary<string, object> init)
        {
            foreach (var obj in init)
            {
                Add(obj.Key, obj.Value);
            }
        }
        public JsonObject(Dictionary<string, string> init, bool isFunction = false)
        {
            foreach (var obj in init)
            {
                Add(obj.Key, obj.Value, isFunction);
            }
        }
        public JsonObject Add(string key, string value, bool isFunction = false)
        {
            _dicData.Add(key, MakeValue(value, isFunction));
            return this;
        }
        public JsonObject Add(string key, object value)
        {
            _dicData.Add(key, MakeValue(value, false));
            return this;
        }
        public dynamic this[string key, bool isFunction = false]
        {
            get
            {
                if (_dicData.ContainsKey(key))
                {
                    return _dicData[key];
                }
                else
                {
                    return new JsonUnknow(key, this);
                }
            }
            set
            {
                if (_dicData.ContainsKey(key))
                {
                    _dicData[key] = MakeValue(value, isFunction);
                }
                else
                {
                    _dicData.Add(key, MakeValue(value, isFunction));
                }
            }
        }
        public string GetKey(int index)
        {
            return _dicData.Keys.ElementAt(index);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var val in _dicData)
            {
                sb.Append(",\"" + val.Key + "\":");
                sb.Append(val.Value.ToString());
            }
            if (_dicData.Count > 0)
            {
                sb.Remove(1, 1);
            }
            sb.Append("}");
            return sb.ToString();
        }
        public int Count
        {
            get
            {
                return _dicData.Count();
            }

        }
        public object GetValue(string key)
        {
            return _dicData[key].GetValue();
        }

        public bool GetBoolean(string key)
        {
            try
            {
                return Convert.ToBoolean(_dicData[key].GetValue());
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string GetString(string key)
        {
            try
            {
                return Convert.ToString(_dicData[key].GetValue());
            }
            catch (Exception)
            {
                return "";
            }
        }
        public decimal GetNumber(string key)
        {
            try
            {
                return Convert.ToDecimal(_dicData[key].GetValue());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool IsNullOrEmpty(string key)
        {
            if (!_dicData.ContainsKey(key)) return true;
            if (_dicData[key].GetValue() == null) return true;
            return string.IsNullOrEmpty(_dicData[key].GetValue().ToString());
        }
        public static implicit operator JsonObject(string json)
        {
            return new JsonObject().Parse(json);
        }
        public bool ExistsKey(string key)
        {
            return _dicData.ContainsKey(key);
        }

        protected override int MakeJson(string json)
        {
            int index = 0;
            skipChar(json, ref index);
            string sChar = json.Substring(index, 1);
            if (sChar == "{")
            {
                for (index++; index < json.Length;)
                {
                    skipChar(json, ref index);
                    sChar = json.Substring(index, 1);
                    if (sChar == "}")
                    {
                        return ++index;
                    }
                    else if (sChar == ",")
                    {
                        index++;
                        skipChar(json, ref index);
                    }
                    else if (index >= json.Length)
                    {
                        throw new Exception("Json对象缺少'｝':" + json.Substring(json.Length - 20));
                    }
                    string sKey = parseKey(json, ref index);
                    this.Add(sKey, MakeValue(json, ref index));
                }
            }
            else
            {
                throw new Exception("无效的Json对象！");
            }
            return index;
        }



        public JsonObject Parse(string json)
        {
            int index = MakeJson(json);
            skipChar(json, ref index);
            if (index < json.Length)
            {
                throw new Exception("第一个Json对象识别成功！后面的Json将忽略！");
            }
            return this;
        }

    }
    public class JsonArray : JsonValue
    {
        private List<JsonValue> _lstData = new List<JsonValue>();
        public JsonArray() { }
        public JsonArray(string json)
        {
            Parse(json);
        }
        public JsonArray(List<object> init)
        {
            foreach (var obj in init)
            {
                Add(obj);
            }
        }
        public JsonArray(List<string> init, bool isFunction = false)
        {
            foreach (var obj in init)
            {
                Add(obj, isFunction);
            }
        }
        public JsonArray Add(string value, bool isFunction = false)
        {
            _lstData.Add(MakeValue(value, isFunction));
            return this;
        }
        public JsonArray Add(object value)
        {
            _lstData.Add(MakeValue(value, false));
            return this;
        }
        public dynamic this[int index, bool isFunction = false]
        {
            get
            {
                if (index == _lstData.Count)
                {
                    return new JsonUnknow(this);
                }
                else
                {
                    return _lstData[index];

                }
            }
            set
            {
                if (index == _lstData.Count)
                {
                    _lstData.Add(MakeValue(value, isFunction));
                }
                else
                {
                    _lstData[index] = MakeValue(value, isFunction);
                }
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var val in _lstData)
            {
                sb.Append(",");
                sb.Append(val.ToString());
            }
            if (_lstData.Count > 0)
            {
                sb.Remove(1, 1);
            }
            sb.Append("]");
            return sb.ToString();
        }
        public int Count
        {
            get
            {
                return _lstData.Count();
            }
        }
        public object GetValue(int index)
        {
            return _lstData[index].GetValue();
        }

        public bool GetBoolean(int index)
        {
            try
            {
                return Convert.ToBoolean(_lstData[index].GetValue());
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string GetString(int index)
        {
            try
            {
                return Convert.ToString(_lstData[index].GetValue());
            }
            catch (Exception)
            {
                return "";
            }
        }
        public decimal GetNumber(int index)
        {
            try
            {
                return Convert.ToDecimal(_lstData[index].GetValue());
            }
            catch (Exception)
            {
                return 0;
            }
        }


        public bool IsNullOrEmpty(int index)
        {
            if (_lstData[index].GetValue() == null) return true;
            return string.IsNullOrEmpty(_lstData[index].GetValue().ToString());
        }
        public static implicit operator JsonArray(string json)
        {
            return new JsonArray().Parse(json);
        }
        protected override int MakeJson(string json)
        {
            int index = 0;
            skipChar(json, ref index);
            string sChar = json.Substring(index, 1);
            if (sChar == "[")
            {
                for (index++; index < json.Length;)
                {
                    skipChar(json, ref index);
                    sChar = json.Substring(index, 1);
                    if (sChar == "]")
                    {
                        return ++index;
                    }
                    else if (sChar == ",")
                    {
                        index++;
                        skipChar(json, ref index);
                    }
                    else if (index >= json.Length)
                    {
                        throw new Exception("Json对象缺少']':" + json.Substring(json.Length - 20));
                    }
                    this.Add(MakeValue(json, ref index));
                }
            }
            else
            {
                throw new Exception("无效的Json数组！");
            }
            return index;
        }

        public JsonArray Parse(string json)
        {
            int index = MakeJson(json);
            skipChar(json, ref index);
            if (index < json.Length)
            {
                throw new Exception("第一个Json数组识别成功！后面的Json将忽略！");
            }
            return this;
        }

    }
    public class JsonUnknow
    {
        private string _key;
        private JsonValue _parent;
        internal JsonUnknow(string key, JsonValue parent)
        {
            _key = key;
            _parent = parent;
        }
        internal JsonUnknow(JsonValue parent)
        {
            _parent = parent;
        }
        public dynamic this[string key, bool isFunction = false]
        {
            get
            {
                return CreateJsonObject()[key];
            }
            set
            {
                CreateJsonObject()[key, isFunction] = value;

            }
        }
        public dynamic this[int index, bool isFunction = false]
        {
            get
            {
                return CreateJsonArray()[index];
            }
            set
            {

                CreateJsonArray()[index, isFunction] = value;

            }
        }

        public JsonArray Add(object item)
        {
            return CreateJsonArray().Add(item);
        }
        public JsonObject Add(string key, object value)
        {
            return CreateJsonObject().Add(key, value);
        }
        private JsonObject CreateJsonObject()
        {
            JsonObject result = new JsonObject();
            if (_key == null)
            {
                (_parent as JsonArray).Add(result);
            }
            else
            {
                (_parent as JsonObject).Add(_key, result);
            }
            return result;
        }
        private JsonArray CreateJsonArray()
        {
            JsonArray result = new JsonArray();
            if (_key == null)
            {
                (_parent as JsonArray).Add(result);
            }
            else
            {
                (_parent as JsonObject).Add(_key, result);
            }
            return result;
        }


    }

}
