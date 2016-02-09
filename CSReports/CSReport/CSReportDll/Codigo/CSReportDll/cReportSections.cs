using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportSections : cIReportGroupSections
    {

        private const String C_MODULE = "cReportSections";

        private Dictionary<String, cReportSection> m_coll = new Dictionary<String, cReportSection>();
        private List<String> m_keys = new List<String>();

        // it is a reference to the controls collection of cReport
        //
        private cReportControls2 m_copyColl;
        private csRptTypeSection m_typeSection;
        private csRptTypeSection m_mainTypeSection;

        // Creates an empty collection.
        public cReportSections()
        {
        }

        public csRptTypeSection getTypeSection()
        {
            return m_typeSection;
        }

        public void setTypeSection(csRptTypeSection rhs)
        {
            m_typeSection = rhs;
        }

        internal void setMainTypeSection(csRptTypeSection rhs)
        {
            m_mainTypeSection = rhs;
        }

        internal void setCopyColl(cReportControls2 rhs)
        {
            m_copyColl = rhs;

            if (m_coll != null) 
            {
                cReportSection section = null;

                for (int _i = 0; _i < this.count(); _i++)
                {
                    section = item(_i);
                    section.setCopyColl(rhs);
                }
            }
        }

		public cReportSection add()
		{
			return add(null, "");
		}
        public cReportSection add(cReportSection c, String key)
        {
            return add(c, key, -1);
        }

        public cReportSection add(cReportSection c, String key, int index)
        {
            try
            {
                if (c == null)
                {
                    c = new cReportSection();
                }
                if (key == "")
                {
                    key = cReportGlobals.getNextKey().ToString();
                }
                else
                {
                    cReportGlobals.refreshNextKey(key);
                }

                key = cReportGlobals.getKey(key);

                if ((index != -1) && this.count() > 0)
                {
                    m_keys.Insert(index, key);
                }
                else
                {
                    m_keys.Add(key);
                }

                m_coll.Add(key, c);
                c.setCopyColl(m_copyColl);

                if (this.count() == 1)
                {
                    c.setTypeSection(m_mainTypeSection);
                }
                else
                {
                    c.setTypeSection(m_typeSection);
                }

                pRefreshIndex();

                c.setIndex(this.count()-1);
                c.setKey(key);

                return c;
            }
            catch
            {
                return null;
            }
        }

        public void clear()
        {
            try
            {
                int n = this.count();
                for (int i = 0; i < n; i++)
                {
                    remove(0);
                }
                return;
            }
            catch
            {
            }
        }

        public void remove(String key)
        {
            try
            {
                item(key).getSectionLines().clear();
                m_coll.Remove(key);
                m_keys.Remove(key);

                for (int i = 0; i < this.count(); i++)
                {
                    m_coll[m_keys[i]].setIndex(i);
                    m_coll[m_keys[i]].setName(m_coll[m_keys[i]].getName().Substring(0, 2).Replace("_", "") 
                                                + "_" + i.ToString());
                }
                return;
            }
            catch
            {
            }
        }

        public void remove(int index)
        {
            try
            {
                item(index).getSectionLines().clear();
                m_coll.Remove(m_keys[index]);
                m_keys.RemoveAt(index);

                for (int i = 0; i < this.count(); i++)
                {
                    cReportSection sec = (cReportSection)m_coll[m_keys[i]];
                    sec.setIndex(i);
                    sec.setName(sec.getName().Substring(0, 2).Replace("_", "")
                                + "_" + i.ToString());
                }
                return;
            }
            catch
            {
            }
        }

        public int count()
        {
            return m_coll.Count;
        }

        public cReportSection item(String key)
        {
            try
            {
                return (cReportSection)m_coll[key];
            }
            catch
            {
                return null;
            }
        }

        public cReportSection item(int index)
        {
            try
            {
                return (cReportSection)m_coll[m_keys[index]];
            }
            catch
            {
                return null;
            }
        }

        private void pRefreshIndex()
        {
            for (int i = 0; i < this.count(); i++)
            {
                item(i).setRealIndex(i);
            }
        }

    }

}
