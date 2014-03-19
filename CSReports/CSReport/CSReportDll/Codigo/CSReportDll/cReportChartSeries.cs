using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportChartSeries : NameObjectCollectionBase
    {

        // Creates an empty collection.
        public cReportChartSeries()
        {
        }

        // Adds elements from an IDictionary into the new collection.
        public cReportChartSeries(IDictionary d, Boolean bReadOnly)
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

        public cReportChartSerie add(cReportChartSerie c, String key)
        {
            try
            {
                if (c == null) 
                { 
                    c = new cReportChartSerie(); 
                }

                if (key == "")
                {
                    Add(getDummyKey(), c);
                }
                else
                {
                    Add(cReportGlobals.getKey(key), c);
                }

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

        public cReportChartSerie item(String key)
        {
            try
            {
                return (cReportChartSerie)this.BaseGet(key);
            }
            catch
            {
                return null;
            }
        }

        public cReportChartSerie item(int index)
        {
            try
            {
                return (cReportChartSerie)this.BaseGet(index);
            }
            catch
            {
                return null;
            }
        }

        private String getDummyKey()
        {
            return "dummy_key_" + this.Count.ToString();
        }

    }
}
