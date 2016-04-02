using System;
using CSKernelClient;

namespace CSXml
{


    public class cXmlProperty
    {
        private const string c_module = "cXmlProperty";

        private string m_name = "";
        private string m_value = "";
        private string m_parent = "";
        private object m_binaryValue = null;

        public object binaryValue
        {
            get { return m_binaryValue; }
            set
            {
                if (value == null)
                {
                    m_binaryValue = null;
                }
                else
                {
                    Type t = value.GetType();
                    if (t.IsArray)
                    {
                        byte[] valueArray = (byte[])value;
                        byte[] newArray = new byte[valueArray.Length];
                        Array.Copy(valueArray, newArray, valueArray.Length);
                        m_binaryValue = newArray;
                    }
                    else
                    {
                        m_binaryValue = null;
                    }
                }
            }
        }

        public string name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public string getName()
        {
            return m_name;
        }

        public void setName(string value)
        {
            m_name = value;
        }

        public int getValueInt(eTypes type)
        {
            return Convert.ToInt32(getValue(type));
        }

        public string getValueString(eTypes type)
        {
            return (string)getValue(type);
        }

        public bool getValueBool(eTypes type)
        {
            return ((int)getValue(type) != 0);
        }

        public object getValue(eTypes type)
        {
            switch (type)
            {
                case eTypes.eBoolean:
                    switch (m_value.ToLower())
                    {
                        case "true":
                        case "verdadero":
                        case "-1":
                        case "1":
                            return -1;
                        //"False":
                        //"Falso":
                        // or any other value is FALSE
                        default:
                            return 0;
                    }
                case eTypes.eDate:
                case eTypes.eDateOrNull:
                    if (cDateUtils.isDate(m_value))
                    {
                        return m_value;
                    }
                    else
                    {
                        return 0;
                    }
                case eTypes.eLong:
                case eTypes.eInteger:
                case eTypes.eId:
                case eTypes.eSingle:
                case eTypes.eCurrency:
                    double dummy;
                    if (double.TryParse(m_value, out dummy))
                    {
                        return m_value;
                    }
                    else
                    {
                        return 0;
                    }
                case eTypes.eText:
                case eTypes.eVariant:
                case eTypes.eCuit:
                    return m_value;
                default:
                    return m_value;
            }
        }

        public void setValue(eTypes type, object value)
        {
            if (type == eTypes.eBoolean)
            {
                m_value = (bool)value ? "-1" : "0";
            }
            else if (type == eTypes.eInteger)
            {
                m_value = Convert.ToInt64(value).ToString();
            }
            else
            {
                m_value = value.ToString();
            }
        }

        public void setValue(object value)
        {
            Type t = value.GetType();
            if (typeof(bool) == t)
            {
                m_value = (bool)value ? "-1" : "0";
            }
            else
            {
                m_value = value.ToString();
            }
        }

        public byte[] getBinaryValue()
        {
            return (byte[])m_binaryValue; 
        }

        public void setBinaryValue(byte[] value)
        {
            binaryValue = value;
        }

        public string parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

    }

}
