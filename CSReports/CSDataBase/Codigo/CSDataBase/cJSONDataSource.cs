using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSDataBase
{
    public class cJSONDataSource
    {
        private string m_name;
        private JObject m_data;

        public cJSONDataSource(string name, JObject data)
        {
            m_name = name;
            m_data = data;
        }

        public string getName()
        {
            return m_name;
        }

        public JObject getData()
        {
            return m_data;
        }
    }
}
