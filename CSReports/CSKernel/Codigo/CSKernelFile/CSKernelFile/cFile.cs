using System;
using System.IO;
using System.Windows.Forms;
using CSKernelClient;


namespace CSKernelFile
{


    public class cFile
    {
        private const string c_module = "cFile";

        private const string c_sep_dir = @"\"; 		// Directory separator character
        private const string c_sep_diralt = @"/";	// Alternate directory separator character

        private FileStream m_file = null;
        private BinaryReader m_br = null;
        private BinaryWriter m_bw = null;
        TextReader m_tr = null;
        private string m_function = "";
        private string m_module = "";
        private bool m_open = false;
        private string m_curPath = "";
        private string m_name = "";
        private string m_path = "";
        private object m_commDialog = null;
        private string m_filter = "";

        public bool isEof
        {
            get
            {
                if (!m_open)
                {
                    return true;
                }
                else
                {
                    if (m_file.Length == m_file.Position)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }

        public string filter
        {
            get { return m_filter; }
            set { m_filter = value; }
        }

        public void setFilter(string value)
        {
            m_filter = value;
        }

        public string getName()
        {
            return m_name;
        }

        public string getPath()
        {
            return m_path;
        }

        public string name
        {
            get { return m_name; }
        }

        public string path
        {
            get { return m_path; }
        }

        public string fullName
        {
            get { return m_path + Path.DirectorySeparatorChar + m_name; }
        }

        public void init(string function, string module, object commDialog)
        {
            m_function = function;
            m_module = module;
            m_commDialog = commDialog;
        }

        public bool open(string fullFileName, eFileMode mode)
        {
            return open(fullFileName, mode, false, true, eFileAccess.eShared, false, false);
        }
        public bool open(string fullFileName, eFileMode mode,
                         bool createFile)
        {
            return open(fullFileName, mode, true, true, eFileAccess.eShared, false, false);
        }
        public bool open(string fullFileName, eFileMode mode,
                         bool createFile, bool silens, eFileAccess access,
                         bool withDialog, bool canOpenOther)
        {
            bool exists = false;
            close();

            if (fullFileName.Length > 0)
            {
                FileInfo fi = new FileInfo(fullFileName);
                exists = (fi.Exists);
            }
            else
            {
                fullFileName = " ";
                exists = false;
            }
            if ((!exists && !createFile) || withDialog)
            {
                exists = fileExists(m_curPath + Path.DirectorySeparatorChar + getFileName(fullFileName));

                if (exists && !withDialog)
                {
                    fullFileName = m_curPath + Path.DirectorySeparatorChar + getFileName(fullFileName);
                }
                else if (silens)
                {
                    return false;
                }
                else if (!userSearchFile(ref fullFileName, false, "Open file", false, canOpenOther))
                {
                    return false;
                }
            }

            if (createFile)
            {
                FileInfo fi = new FileInfo(fullFileName);
                if (fi.Exists)
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch (Exception ex)
                    {
                        cError.mngError(ex, "open", c_module, "");
                        return false;
                    }
                }
            }
            try
            {
                switch (mode)
                {
                    case eFileMode.eAppend:
                        switch (access)
                        {
                            case eFileAccess.eShared:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.Append,
                                                        FileAccess.Write,
                                                        FileShare.ReadWrite);
                                break;
                            case eFileAccess.eLockWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.Append,
                                                        FileAccess.Write,
                                                        FileShare.Read);
                                break;
                            case eFileAccess.eLockReadWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.Append,
                                                        FileAccess.Write,
                                                        FileShare.None);
                                break;
                            default:
                                return false;
                        }
                        break;
                    // text mode
                    case eFileMode.eWrite:
                        switch (access)
                        {
                            case eFileAccess.eShared:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Write,
                                                        FileShare.ReadWrite);
                                break;
                            case eFileAccess.eLockWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Write,
                                                        FileShare.Read);
                                break;
                            case eFileAccess.eLockReadWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Write,
                                                        FileShare.None);
                                break;
                            default:
                                return false;
                        }
                        break;
                    case eFileMode.eRead:
                        switch (access)
                        {
                            case eFileAccess.eShared:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Read,
                                                        FileShare.ReadWrite);
                                break;
                            case eFileAccess.eLockWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Read,
                                                        FileShare.Read);
                                break;
                            case eFileAccess.eLockReadWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Read,
                                                        FileShare.None);
                                break;
                            default:
                                return false;
                        }
                        break;
                    // binary mode
                    case eFileMode.eBinaryWrite:
                        switch (access)
                        {
                            case eFileAccess.eShared:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Write,
                                                        FileShare.ReadWrite);
                                m_bw = new BinaryWriter(m_file);
                                break;
                            case eFileAccess.eLockWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Write,
                                                        FileShare.Read);
                                m_bw = new BinaryWriter(m_file);
                                break;
                            case eFileAccess.eLockReadWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Write,
                                                        FileShare.None);
                                m_bw = new BinaryWriter(m_file);
                                break;
                            default:
                                return false;
                        }
                        break;
                    case eFileMode.eBinaryRead:
                        switch (access)
                        {
                            case eFileAccess.eShared:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Read,
                                                        FileShare.ReadWrite);
                                m_br = new BinaryReader(m_file);
                                break;
                            case eFileAccess.eLockWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Read,
                                                        FileShare.Read);
                                m_br = new BinaryReader(m_file);
                                break;
                            case eFileAccess.eLockReadWrite:
                                m_file = new FileStream(fullFileName,
                                                        FileMode.OpenOrCreate,
                                                        FileAccess.Read,
                                                        FileShare.None);
                                m_br = new BinaryReader(m_file);
                                break;
                            default:
                                return false;
                        }
                        break;
                    default:
                        return false;
                }
                m_open = true;
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "open", c_module, "");
                return false;
            }
        }

        public bool save(string fullFileName,
                         out bool exists,
                         out bool readOnly,
                         string description)
        {
            exists = false;
            readOnly = false;

            if (fullFileName.Length == 0)
            {
                fullFileName = " ";
            }
            if (userSearchFile(ref fullFileName, true, description, true, false))
            {
                if (fullFileName.Length > 0)
                {
                    FileInfo fi = new FileInfo(fullFileName);
                    exists = fi.Exists;
                    if (exists)
                    {
                        if ((fi.Attributes & FileAttributes.Normal
                             | fi.Attributes & FileAttributes.ReadOnly
                             | fi.Attributes & FileAttributes.Archive) != 0)
                        {
                            if ((fi.Attributes & FileAttributes.ReadOnly) != 0)
                            {
                                readOnly = true;
                            }
                        }
                        else
                        {
                            exists = false;
                        }
                    }
                }
                else
                {
                    fullFileName = " ";
                    exists = false;
                }
            }
            return true;
        }

        public bool write(string text)
        {
            if (!m_open) return false;
            try
            {
                TextWriter tw = new StreamWriter(m_file);
                tw.WriteLine(text);
                tw.Close();
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "write", c_module, "failed writing text to file: " + m_path + Path.DirectorySeparatorChar + m_name);
                return false;
            }
        }

        public bool read(out string text, out bool eof)
        {
            text = "";
            eof = false;
            if (!m_open) return false;
            try
            {

                if (m_tr == null)
                {
                    m_tr = new StreamReader(m_file);
                }

                text = m_tr.ReadLine();
                if (text == null)
                {
                    eof = true;
                    text = "";
                    m_tr.Close();
                    m_tr = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "read", c_module, "failed reading text from file: " + m_path + Path.DirectorySeparatorChar + m_name);
                return false;
            }
        }

        public bool binaryWrite(byte[] buffer)
        {
            if (!m_open) return false;
            try
            {
                m_bw.Write(buffer);
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "binaryWrite", c_module, "failed writing in binary mode to file: " + m_path + Path.DirectorySeparatorChar + m_name);
                return false;
            }
        }

        public bool binaryRead(out byte[] buffer, out bool eof)
        {
            buffer = null;
            eof = false;
            if (!m_open) return false;
            try
            {
                if (isEof)
                {
                    eof = true;
                    buffer = null;
                }
                else
                {
                    long bytesInFile = m_file.Length - m_file.Position;
                    if (bytesInFile < buffer.Length)
                    {
                        buffer = new byte[bytesInFile];
                    }
                    buffer = m_br.ReadBytes(buffer.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "binaryRead", c_module, "failed reading in binary mode from file: " + m_path + Path.DirectorySeparatorChar + m_name);
                return false;
            }
        }

        public void close()
        {
            try
            {
                if (m_file != null)
                {
                    m_file.Close();
                    if (m_br != null)
                    {
                        m_br.Close();
                        m_br = null;
                    }
                    if (m_bw != null)
                    {
                        m_bw.Close();
                        m_bw = null;
                    }
                    m_file = null;
                }
                m_open = false;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "binaryRead", c_module, "failed reading in binary mode from file: " + m_path + Path.DirectorySeparatorChar + m_name);
            }
        }

        public bool userSearchFile(ref string fullFileName,
                                   bool ifNotExistsIsOk,
                                   string description,
                                   bool saving,
                                   bool canOpenOther)
        {
            string userFile = "";
            bool extValid = false;
            bool nameValid = false;
            bool exists = false;

            do
            {

                if (showOpenFileDlg(out userFile,
                                    getFileExt(fullFileName),
                                    getFileName(fullFileName),
                                    getPath(fullFileName),
                                    description,
                                    saving))
                {

                    exists = fileExists(userFile);

                    if (exists || ifNotExistsIsOk)
                    {
                        if (fullFileName == " " || getFileWithoutExt(fullFileName) == "*")
                        {
                            nameValid = true;
                        }
                        else if (ifNotExistsIsOk
                                || canOpenOther
                                || getFileWithoutExt(fullFileName) == getFileWithoutExt(userFile))
                        {
                            nameValid = true;
                        }
                        if (nameValid)
                        {
                            if (fullFileName == " " || getFileExt(fullFileName) == "*")
                            {
                                extValid = true;
                            }
                            else if (ifNotExistsIsOk || getFileExt(fullFileName) == getFileExt(userFile))
                            {
                                extValid = true;
                            }
                            if (extValid && nameValid && (exists || ifNotExistsIsOk))
                            {
                                break;
                            }
                            else
                            {
                                nameValid = false;
                                extValid = false;
                            }
                        }

                    }
                }
                else
                {
                    return false;
                }

            } while (true);

            m_curPath = getPath(userFile);
            fullFileName = userFile;
            m_name = getFileName(userFile);
            m_path = getPath(userFile);

            return true;
        }

        public bool showOpenFileDlg(out string userFile,
                                    string filter,
                                    string fileToSearch,
                                    string curDir,
                                    string title,
                                    bool saving)
        {
            userFile = "";
            FileDialog fd = m_commDialog as FileDialog;
            if (curDir.Length > 0 && curDir != " ")
            {
                DirectoryInfo di = new DirectoryInfo(curDir);
                if (di.Exists)
                {
                    fd.InitialDirectory = curDir;
                }
            }
            if (fileToSearch != " ." && (fileToSearch.Length < 2 || fileToSearch.Substring(0, 2) != "*."))
            {
                fd.FileName = fileToSearch;
            }
            else
            {
                fd.FileName = "";
            }
            if (m_filter.Length > 0)
            {
                fd.Filter = m_filter;
            }
            else if (filter.Length > 0)
            {
                fd.Filter = filter;
            }
            fd.Title = title;
            if (saving)
            {
                SaveFileDialog fs = m_commDialog as SaveFileDialog;
                if (fs.ShowDialog() == DialogResult.OK)
                {
                    userFile = fs.FileName;
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            else
            {
                OpenFileDialog fc = m_commDialog as OpenFileDialog;
                if (fc.ShowDialog() == DialogResult.OK)
                {
                    userFile = fc.FileName;
                    return true;
                }
                else 
                {
                    return false;
                }
            }
        }

        public static string getFileName(string fullFileName)
        {
            return getFileWithoutExt(fullFileName) + "." + getFileExt(fullFileName);
        }

        public static string getFileExt(string fullFileName)
        {
            string path = "";
            string fileName = "";
            int sepPos = 0;
            string sep = "";

            getPathAndFileName(fullFileName, out path, out fileName);
            sepPos = fileName.Length;

            if (sepPos == 0)
            {
                return "";
            }
            else
            {
                sepPos -= 1;
                sep = fileName.Substring(sepPos, 1);
                while (sep != ".")
                {
                    sepPos--;
                    if (sepPos < 0) break;
                    sep = fileName.Substring(sepPos, 1);
                }
                if (sepPos < 0)
                {
                    return "";
                }
                else
                {
                    return fileName.Substring(sepPos + 1);
                }
            }
        }

        public static string getFileWithoutExt(string fullFileName)
        {
            string path = "";
            string fileName = "";
            int sepPos = 0;
            string sep = "";

            getPathAndFileName(fullFileName, out path, out fileName);
            sepPos = fileName.Length;

            if (sepPos == 0)
            {
                return fileName;
            }

            sepPos -= 1;
            sep = fileName.Substring(sepPos, 1);
            while (sep != ".")
            {
                sepPos--;
                if (sepPos < 0) break;
                sep = fileName.Substring(sepPos, 1);
            }
            if (sepPos < 0)
            {
                return fileName;
            }
            else
            {
                return fileName.Substring(0, sepPos);
            }
        }

        public static string getPath(string fullFileName)
        {
            string path = "";
            string fileName = "";

            getPathAndFileName(fullFileName, out path, out fileName);
            return path;
        }

        public static void getPathAndFileName(string fullFileName,
                                              out string path,
                                              out string fileName)
        {
            int sepPos = 0;
            string sep = "";

            sepPos = fullFileName.Length;
            if (sepPos == 0)
            {
                path = "";
                fileName = "";
            }
            else
            {
                sepPos -= 1;
                sep = fullFileName.Substring(sepPos, 1);
                while (!isSeparator(sep))
                {
                    sepPos--;
                    if (sepPos < 0) break;
                    sep = fullFileName.Substring(sepPos, 1);
                }
                if (sepPos == fullFileName.Length - 1)
                {
                    // case when fullFileName is c:\ or d:\ etc.
                    path = fullFileName.Substring(0, sepPos);
                    fileName = fullFileName;
                }
                else if (sepPos < 0)
                {
                    // case when fullFileName is c: or d: etc.
                    path = fullFileName;
                    fileName = fullFileName;
                }
                else
                {
                    path = fullFileName.Substring(0, sepPos);
                    fileName = fullFileName.Substring(sepPos + 1);
                }
            }
        }

        public static bool copyFile(string fullFileNameSource, string fullFileNameDestination)
        {
            try
            {
                File.Copy(fullFileNameSource, fullFileNameDestination);
                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "copyFile", c_module, "failed copying [" + fullFileNameSource + "] to [" + fullFileNameDestination + "]");
                return false;
            }
        }

        private static bool isSeparator(string character)
        {
            switch (character)
            {
                case c_sep_dir:
                    return true;
                case c_sep_diralt:
                    return true;
                default:
                    return false;
            }
        }

        private static bool fileExists(string fullFileName) 
        {
            try
            {
                if (fullFileName == "\\ .")
                {
                    return false;
                }
                else 
                {
                    FileInfo fi = new FileInfo(fullFileName);
                    return fi.Exists;                
                }
            }
            catch (Exception ex) {
                return false;
            }
        }

        public cFile()
        {
        }
    }
}
