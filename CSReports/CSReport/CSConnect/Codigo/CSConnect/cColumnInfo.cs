using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CSKernelClient;

namespace CSConnect
{

    public class cColumnInfo
    {

        private const String C_MODULE = "cColumnInfo";

        private String m_name = "";
        private CSDataBase.csDataType m_columnType;

        // TODO: remove me
        // private String m_value = "";
        private int m_position = 0;
        private String m_key = "";

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

        public CSDataBase.csDataType getColumnType()
        {
            return m_columnType;
        }

        public void setColumnType(CSDataBase.csDataType rhs)
        {
            m_columnType = rhs;
        }
        // TODO: remove me
        /*
        public String getValue()
        {
            return m_value;
        }

        public void setValue(String rhs)
        {
            m_value = rhs;
        }
        */
        public int getPosition()
        {
            return m_position;
        }

        public void setPosition(int rhs)
        {
            m_position = rhs;
        }
    }

}
