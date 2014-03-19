using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportPageSettings : NameObjectCollectionBase
    {

        // Creates an empty collection.
        public cReportPageSettings()
        {
        }

        // Adds elements from an IDictionary into the new collection.
        public cReportPageSettings(IDictionary d, Boolean bReadOnly)
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
        public void Remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        public void Clear()
        {
            this.BaseClear();
        }

        // Removes an entry with the specified key from the collection.
        public void remove(String key)
        {
            this.BaseRemove(key);
        }

        // Removes an entry in the specified index from the collection.
        public void remove(int index)
        {
            this.BaseRemoveAt(index);
        }

        // Clears all the elements in the collection.
        public void clear()
        {
            this.BaseClear();
        }

        private float m_height = 0;
        
        public float getHeight()
        {
            return m_height;
        }

        public void setHeight(float rhs)
        {
            m_height = rhs;
        }

        public cReportPageInfo add(
            cReportSectionLine sectionLine,
            cReportPageInfo c,
            String key)
        {
            try
            {
                if (c == null) 
                { 
                    c = new cReportPageInfo(); 
                }
                if (key == "")
                {
                    key = cReportGlobals.getNextKey().ToString();
                }

                key = cReportGlobals.getKey(key);
                Add(key, c);
                c.setSectionLine(sectionLine);
                return c;
            }
            catch
            {
                return null;
            }
        }

        public int count()
        {
            return this.Count;
        }

        public cReportPageInfo item(String key)
        {
            try
            {
                return (cReportPageInfo)this.BaseGet(key);
            }
            catch
            {
                return null;
            }
        }

        public cReportPageInfo item(int index)
        {
            try
            {
                return (cReportPageInfo)this.BaseGet(index);
            }
            catch
            {
                return null;
            }
        }

    }

}
