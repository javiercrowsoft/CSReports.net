using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportGroups : NameObjectCollectionBase
    {

        // Creates an empty collection.
        public cReportGroups()
        {
        }

        // Adds elements from an IDictionary into the new collection.
        public cReportGroups(IDictionary d, Boolean bReadOnly)
        {
            foreach (DictionaryEntry de in d)
            {
                this.BaseAdd((String)de.Key, de.Value);
            }
            this.IsReadOnly = bReadOnly;
        }

        // Gets a key-and-value pair (DictionaryEntry) using an index.
        public DictionaryEntry this[int index]
        {
            get
            {
                return (new DictionaryEntry(
                    this.BaseGetKey(index), this.BaseGet(index)));
            }
        }

        // Gets or sets the value associated with the specified key.
        public Object this[String key]
        {
            get
            {
                return (this.BaseGet(key));
            }
            set
            {
                this.BaseSet(key, value);
            }
        }

        // Gets a String array that contains all the keys in the collection.
        public String[] AllKeys
        {
            get
            {
                return (this.BaseGetAllKeys());
            }
        }

        // Gets an Object array that contains all the values in the collection.
        public Array AllValues
        {
            get
            {
                return (this.BaseGetAllValues());
            }
        }

        // Gets a String array that contains all the values in the collection.
        public String[] AllStringValues
        {
            get
            {
                return ((String[])this.BaseGetAllValues(typeof(String)));
            }
        }

        // Gets a value indicating if the collection contains keys that are not null.
        public Boolean HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }

        // Adds an entry to the collection.
        public void Add(String key, Object value)
        {
            this.BaseAdd(key, value);
        }

        // Removes an entry with the specified key from the collection.
        private void Remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        private void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        private void Clear()
        {
            this.BaseClear();
        }

        private cReportSections m_groupsHeaders = new cReportSections();
        private cReportSections m_groupsFooters = new cReportSections();

        public cReportSections getGroupsHeaders()
        {
            return m_groupsHeaders;
        }

        internal void setGroupsHeaders(cReportSections rhs)
        {
            m_groupsHeaders = rhs;
        }

        public cReportSections getGroupsFooters()
        {
            return m_groupsFooters;
        }

        internal void setGroupsFooters(cReportSections rhs)
        {
            m_groupsFooters = rhs;
        }

        public cReportGroup add(cReportGroup c, String key) 
        {
            try 
            {
                if (c == null) 
                { 
                    c = new cReportGroup(); 
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
                Add(key, c);

                c.setKey(key);
                c.setIndex(count());

                c.setHeader(m_groupsHeaders.add(null, "", -1));
                c.setFooter(m_groupsFooters.add(null, "", 1));

                pSetName(c, "G_" + c.getIndex().ToString());
                pSetName(c.getHeader(), c.getName());
                pSetName(c.getFooter(), c.getName());

                c.getHeader().setTypeSection(csRptTypeSection.CSRPTTPGROUPHEADER);
                c.getFooter().setTypeSection(csRptTypeSection.CSRPTTPGROUPFOOTER);

                return c;
            } 
            catch 
            {
                return null;
            }
        }

        public cReportGroup add2(cReportGroup c, String key) 
        {
            try 
            {
                if (c == null) 
                { 
                    c = new cReportGroup(); 
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
                Add(key, c);

                c.setKey(key);
                c.setIndex(this.Count);

                m_groupsHeaders.add(c.getHeader(), "", -1);
                m_groupsFooters.add(c.getFooter(), "", 1);

                pSetName(c, "G_" + c.getIndex().ToString());
                pSetName(c.getHeader(), c.getName());
                pSetName(c.getFooter(), c.getName());

                c.getHeader().setTypeSection(csRptTypeSection.CSRPTTPGROUPHEADER);
                c.getFooter().setTypeSection(csRptTypeSection.CSRPTTPGROUPFOOTER);

                return c;
            } 
            catch
            {
                return null;
            }
        }

        private void pSetName(cReportGroup c, String name)
        {
            c.setName(pSetName(c.getName(), name));
        }
        private void pSetName(cReportSection c, String name)
        {
            c.setName(pSetName(c.getName(), name));
        }
        private String pSetName(String section, String name)
        {
            String sectionName = section.ToLower();
            if (sectionName.Substring(0, 5) == "group"
                || sectionName.Substring(0, 5) == "grupo"
                || sectionName.Substring(0, 3) == "gh_"
                || sectionName.Substring(0, 3) == "gf_"
                || sectionName.Substring(0, 2) == "g_"
                || sectionName.Length == 0)
            {
                return name;
            }
            else
            {
                return section;
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
                String keyH = "";
                String keyF = "";

                keyH = m_groupsHeaders.item(item(key).getHeader().getKey()).getKey();
                keyF = m_groupsFooters.item(item(key).getFooter().getKey()).getKey();

                m_groupsHeaders.remove(keyH);
                m_groupsFooters.remove(keyF);

                Remove(key);

                // Update the index
                //
                for (int i = 0; i < this.Count; i++)
                {
                    item(i).setIndex(i);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void remove(int index)
        {
            try
            {
                String keyH = "";
                String keyF = "";

                keyH = m_groupsHeaders.item(item(index).getHeader().getKey()).getKey();
                keyF = m_groupsFooters.item(item(index).getFooter().getKey()).getKey();

                m_groupsHeaders.remove(keyH);
                m_groupsFooters.remove(keyF);

                Remove(index);

                // Update the index
                //
                for (int i = 0; i < this.Count; i++)
                {
                    item(i).setIndex(i);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public int count()
        {
            return this.Count;
        }

        public cReportGroup item(String key)
        {
            try
            {
                return (cReportGroup)this.BaseGet(key);
            }
            catch
            {
                return null;
            }
        }

        public cReportGroup item(int index)
        {
            try
            {
                return (cReportGroup)this.BaseGet(index);
            }
            catch
            {
                return null;
            }
        }

    }

}
