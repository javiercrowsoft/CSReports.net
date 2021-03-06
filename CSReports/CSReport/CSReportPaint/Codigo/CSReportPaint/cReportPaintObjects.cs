﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CSKernelClient;

namespace CSReportPaint
{

    public class cReportPaintObjects : NameObjectCollectionBase
    {

        // Creates an empty collection.
        public cReportPaintObjects()
        {
        }

        // Adds elements from an IDictionary into the new collection.
        public cReportPaintObjects(IDictionary d, Boolean bReadOnly)
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
            removeZOrder(key);
        }

        // Removes an entry in the specified index from the collection.
        public void remove(int index)
        {
            string key = item(index).getKey();
            this.BaseRemoveAt(index);
            removeZOrder(key);
        }

        // Clears all the elements in the collection.
        public void clear()
        {
            this.BaseClear();
            m_zorder = null;
        }

        private String[] m_zorder = null;

        internal cReportPaintObject add(cReportPaintObject c, String key)
        {
            try
            {
                if (c == null) 
                { 
                    c = new cReportPaintObject(); 
                }
                if (key == "")
                {
                    key = cGlobals.getNextKey().ToString();
                }

                key = cGlobals.getKey(key);
                Add(key, c);

                c.setKey(key);
                G.redimPreserve(ref m_zorder, this.count());
                m_zorder[this.count()-1] = key;

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

        public void bringToFront(String key)
        {
            zorder(key, true);
        }

        public void sendToBack(String key)
        {
            zorder(key, false);
        }

        // moves the element refered by key to the last position if top is true or
        // to the first position if top is false in m_zorder
        //
        // nZorder == 0 is the heap's bottom and the max nZorder is at 
        // the heap's top 
        //
        public void zorder(String key, bool top)
        {
            int i;

            // first we search the element using key
            //
            for (i = 0; i < m_zorder.Length; i++)
            {
                if (m_zorder[i] == key)
                {
                    break;
                }
            }

            if (i >= m_zorder.Length-1 && top) 
            { 
                return; 
            }
            if (i == 0 && !top) 
            { 
                return; 
            }

            if (top)
            {
                for (; i < m_zorder.Length - 1; i++)
                {
                    m_zorder[i] = m_zorder[i + 1];
                    item(m_zorder[i]).getAspect().setNZOrder(i);
                }
                m_zorder[m_zorder.Length-1] = key;
                item(key).getAspect().setNZOrder(m_zorder.Length-1);
            }
            else
            {
                for (; i > 0; i--)
                {
                    m_zorder[i] = m_zorder[i - 1];
                    item(m_zorder[i]).getAspect().setNZOrder(i);
                }
                m_zorder[0] = key;
                item(key).getAspect().setNZOrder(0);
            }
        }

        public int getZOrderForKey(String key)
        {
            for (int i = 0; i < m_zorder.Length; i++)
            {
                if (m_zorder[i] == key)
                {
                    return i;
                }
            }

            return -1;
        }

        public String getNextKeyForZOrder(int index)
        {
            return m_zorder[index];
        }

        public cReportPaintObject getNextPaintObjForZOrder(int index)
        {
            return item(getNextKeyForZOrder(index));
        }

        public cReportPaintObject item(String key)
        {
            try
            {
                return (cReportPaintObject)this.BaseGet(key);
            }
            catch
            {
                return null;
            }
        }

        public cReportPaintObject item(int index)
        {
            try
            {
                return (cReportPaintObject)this.BaseGet(index);
            }
            catch
            {
                return null;
            }
        }

        private void removeZOrder(String sKey)
        {
            for (int i = 0; i < m_zorder.Length; i++)
            {
                if (m_zorder[i] == sKey)
                {
                    for (int j = i; j < m_zorder.Length - 1; j++)
                    {
                        m_zorder[j] = m_zorder[j + 1];
                    }
                    G.redimPreserve(ref m_zorder, m_zorder.Length - 1);
                    return;
                }
            }
        }

    }

}
