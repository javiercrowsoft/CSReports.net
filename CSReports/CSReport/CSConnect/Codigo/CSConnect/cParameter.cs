using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSConnect
{
    public class cParameter
    {
        private const String C_MODULE = "cParameter";

        private String m_name = "";
        private CSDataBase.csDataType m_typeColumn;
        private String m_value = "";
        private int m_position = 0;
        private String m_key = "";
        private bool m_hasDefault;
        private String m_default = "";
        private bool m_isNullable;
        private int m_maxLength = 0;

        public String getKey()
        {
            return m_key;
        }

        public void setKey(String rhs)
        {
            m_key = rhs;
        }

        public String getName()
        {
            return m_name;
        }

        public void setName(String rhs)
        {
            m_name = rhs;
        }

        public CSDataBase.csDataType getTypeColumn()
        {
            return m_typeColumn;
        }

        public void setTypeColumn(CSDataBase.csDataType rhs)
        {
            m_typeColumn = rhs;
        }

        public String getValue()
        {
            return m_value;
        }

        public void setValue(String rhs)
        {
            m_value = rhs;
        }

        public int getPosition()
        {
            return m_position;
        }

        public void setPosition(int rhs)
        {
            m_position = rhs;
        }

        public bool getHasDefault()
        {
            return m_hasDefault;
        }

        public void setHasDefault(bool rhs)
        {
            m_hasDefault = rhs;
        }

        public String getDefaultValue()
        {
            return m_default;
        }

        public void setDefaultValue(String rhs)
        {
            m_default = rhs;
        }

        public bool getIsNullable()
        {
            return m_isNullable;
        }

        public void setIsNullable(bool rhs)
        {
            m_isNullable = rhs;
        }

        public int getMaxLength()
        {
            return m_maxLength;
        }

        public void setMaxLength(int rhs)
        {
            m_maxLength = rhs;
        }
    }
}
