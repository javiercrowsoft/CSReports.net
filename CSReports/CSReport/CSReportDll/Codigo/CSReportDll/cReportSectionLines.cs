using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{

    public class cReportSectionLines
    {

        // it is a reference to the controls collection of cReport
        //
        private cReportControls2 m_copyColl;
        private csRptTypeSection m_typeSection;
        private Hashtable m_coll = new Hashtable();
        private List<String> m_keys = new List<String>();

        // Creates an empty collection.
        public cReportSectionLines()
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

        internal void setCopyColl(cReportControls2 rhs)
        {
            cReportSectionLine sectionLn = null;
            m_copyColl = rhs;

            for (int _i = 0; _i < this.count(); _i++)
            {
                sectionLn = item(_i);
                sectionLn.setCopyColl(rhs);
            }
        }

        internal cReportControls2 getCopyColl()
        {
            return m_copyColl;
        }

        public cReportSectionLine add(cReportSectionLine c, String key, int index)
        {
            try
            {
                if (c == null) 
                { 
                    c = new cReportSectionLine(); 
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
                c.setTypeSection(m_typeSection);

                pRefreshIndex();
                c.setIndex(this.count());
                c.setKey(key);

                return c;
            }
            catch (Exception ex)
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
                cReportSectionLine w_item = item(key);
                if (w_item != null)
                {
                    if (w_item.getControls() != null)
                    {
                        w_item.getControls().clear();
                        w_item.getControls().setSectionLine(null);
                        w_item.getControls().setCopyColl(null);
                    }
                    m_coll.Remove(key);
                    m_keys.Remove(key);
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
                cReportSectionLine w_item = item(index);
                if (w_item != null)
                {
                    if (w_item.getControls() != null)
                    {
                        w_item.getControls().clear();
                        w_item.getControls().setSectionLine(null);
                        w_item.getControls().setCopyColl(null);
                    }
                    m_coll.Remove(index);
                    m_keys.RemoveAt(index);
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

        public cReportSectionLine item(String key)
        {
            try
            {
                return (cReportSectionLine)m_coll[key];
            }
            catch 
            {
                return null;
            }
        }

        public cReportSectionLine item(int index)
        {
            try
            {
                return (cReportSectionLine)m_coll[m_keys[index]];
            }
            catch
            {
                return null;
            }
        }

        private void pRefreshIndex()
        {
            int i = 0;
            for (i = 1; i <= this.count(); i++)
            {
                item(i).setrealIndex(i);
            }
        }

    }

}
