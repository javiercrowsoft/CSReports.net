using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CSKernelClient;
using CSReportGlobals;

namespace CSReportDll
{
    public class cReportControls : NameObjectCollectionBase , IDisposable
    {

        // Creates an empty collection.
        public cReportControls()
        {
        }

        // Adds elements from an IDictionary into the new collection.
        public cReportControls(IDictionary d, Boolean bReadOnly)
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

        private const String C_MODULE = "cReportControls";

        // it is a reference to the controls collection of cReport
        //
        private cReportControls2 m_copyColl;
        private csRptTypeSection m_typeSection;
        private int[] m_collByLeft;

        // this reference tell in which section line is this controls collection
        //
        private cReportSectionLine m_sectionLine;

        public csRptTypeSection getTypeSection()
        {
            return m_typeSection;
        }

        public void setTypeSection(csRptTypeSection rhs)
        {
            m_typeSection = rhs;
        }

        public cReportControls2 getCopyColl()
        {
            return m_copyColl;
        }

        public void setCopyColl(cReportControls2 rhs)
        {
            m_copyColl = rhs;
        }

        public cReportSectionLine getSectionLine()
        {
            return m_sectionLine;
        }

        public void setSectionLine(cReportSectionLine rhs)
        {
            m_sectionLine = rhs;

            cReportControl ctrl = null;
            for (int _i = 0; _i < this.Count; _i++)
            {
                ctrl = item(_i);
                ctrl.setSectionLine(rhs);
            }
        }

        public int[] getCollByLeft()
        {
            return m_collByLeft;
        }

		public cReportControl add()
		{
			return add(null, "");
		}

        public cReportControl add(cReportControl c, String key)
        {
            try
            {

                if (c == null) 
                { 
                    c = new cReportControl(); 
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
                c.setTypeSection(m_typeSection);
                c.setSectionLine(m_sectionLine);

                if (m_copyColl != null) 
                { 
                    m_copyColl.add2(c, key); 
                }

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
            }
            catch
            {
            }
        }

        public void remove(String key)
        {
            try
            {
                item(key).setSectionLine(null);
                if (m_copyColl != null)
                {
                    m_copyColl.remove(item(key).getKey());
                }
                Remove(key);
            }
            catch
            {
            }
        }

        public void remove(int index)
        {
            try
            {
                item(index).setSectionLine(null);
                if (m_copyColl != null)
                {
                    m_copyColl.remove(item(index).getKey());
                }
                Remove(index);
            }
            catch
            {
            }
        }

        public int count()
        {
            return this.Count;
        }

        public cReportControl item(String key)
        {
            try
            {
                return (cReportControl)this.BaseGet(key);
            }
            catch
            {
                return null;
            }
        }

        public cReportControl item(int index)
        {
            try
            {
                return (cReportControl)this.BaseGet(index);
            }
            catch
            {
                return null;
            }
        }

        public void orderCollByLeft()
        {
            int j = 0;
            int i = 0;
            int tmp = 0;
            cReportControl ctl1 = null;
            cReportControl ctl2 = null;

            G.redim(ref m_collByLeft, this.Count);

            for (i = 0; i < m_collByLeft.Length; i++)
            {
                m_collByLeft[i] = i;
            }

            for (i = 0; i < this.Count; i++)
            {
                for (j = i; j < this.Count; j++)
                {
                    ctl1 = item(m_collByLeft[j]);
                    ctl2 = item(m_collByLeft[j + 1]);

                    if (ctl2.getLabel().getAspect().getLeft() < ctl1.getLabel().getAspect().getLeft())
                    {
                        tmp = m_collByLeft[j];
                        m_collByLeft[j] = m_collByLeft[j + 1];
                        m_collByLeft[j + 1] = tmp;
                    }
                }
            }
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Track whether Dispose has been called.
        private bool disposed = false;

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    releaseReferences();
                }

                // Note disposing has been done.
                disposed = true;

            }
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~cReportControls()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        private void releaseReferences()
        {
            cReportControl ctrl;
            for (int _i = 0; _i < this.Count; _i++)
            {
                ctrl = item(_i);
                ctrl.setSectionLine(null);
            }
        }

    }

}
