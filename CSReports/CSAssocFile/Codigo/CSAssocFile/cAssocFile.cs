using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

namespace CSAssocFile
{
    public class cAssocFile
    {

        [DllImport("shell32.dll", EntryPoint="FindExecutable")] 
            public static extern long FindExecutableA(string lpFile, string lpDirectory, StringBuilder lpResult);

        [DllImport("shell32.dll")]
        static extern void SHChangeNotify(HChangeNotifyEventID wEventId,
                                           HChangeNotifyFlags uFlags,
                                           IntPtr dwItem1,
                                           IntPtr dwItem2);

        private const String C_CROWSOFTKEY_EXTENSIONS = "SOFTWARE\\CrowSoft\\Extensions";
        private String m_question;
        private String m_yesButton;
        private String m_noButton;
        private String m_dontAsk;

        public String question
        {
            set
            {
                m_question = value;
            }
        }

        public String yesButton
        {
            set
            {
                m_yesButton = value;
            }
        }

        public String noButton
        {
            set
            {
                m_noButton = value;
            }
        }

        public String dontAsk
        {
            set
            {
                m_dontAsk = value;
            }
        }

        public String getLongFileName(String fullFileName)
        {
            return Path.GetFullPath(fullFileName);
        }

        public String getAssociatedApp(String fullFileName)
        {
            return findExecutable(fullFileName);
        }

        private string findExecutable(string pv_strFilename)
        {
            StringBuilder objResultBuffer = new StringBuilder(1024);
            long lngResult = 0;

            lngResult = FindExecutableA(pv_strFilename, string.Empty, objResultBuffer);

            if(lngResult >= 32)
            {
                return objResultBuffer.ToString();
            }
            else
            {
                return string.Format("Error: ({0})", lngResult);
            }
        }

        public void associateFileExtension(
            String extension, 
            String pathToExecute,
            String applicationName)
        {
            //' extension is three letters without the "."
            //' pathToExecute is full path to exe file
            //' application Name is any name you want as description of Extension

            String sKeyName;        // Holds Key Name in registry.
            String sKeyValue;       // Holds Key Value in registry.
    
            if (!extension.Contains("."))
            {
                //' This creates a Root entry for the extension to be associated with ' ApplicationName' .
                sKeyName = "." + extension;
                sKeyValue = applicationName;
                RegistryKey rKey = Registry.ClassesRoot.CreateSubKey(sKeyName);
                rKey.SetValue(sKeyName, sKeyValue);

                //' This creates a Root entry called ' ApplicationName' .
                sKeyName = applicationName;
                sKeyValue = applicationName;
                RegistryKey rKeyApp = Registry.ClassesRoot.CreateSubKey(sKeyName);
                rKeyApp.SetValue(sKeyName, sKeyValue);
    
                //' This sets the command line for ' ApplicationName' .
                sKeyName = applicationName;
                sKeyValue = "\"" + pathToExecute + "\" %1";
                rKey = rKeyApp.CreateSubKey("shell\\open\\command");
                rKey.SetValue(sKeyName, sKeyValue);
    
                //' This sets the default icon
                sKeyName = applicationName;
                sKeyValue = "\"" + pathToExecute + "\",0";
                rKey = rKeyApp.CreateSubKey("DefaultIcon");
                rKey.SetValue(sKeyName, sKeyValue);

                SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public void unAssociateFileExtension(
            String extension, 
            String applicationName)
        {
            String sKeyName;   // Finds Key Name in registry.

            if (!extension.Contains("."))
            {
                //' This deletes the default icon
                sKeyName = applicationName;
                Registry.ClassesRoot.DeleteSubKey(sKeyName + "\\DefaultIcon");

                //' This deletes the command line for "ApplicationName".
                Registry.ClassesRoot.DeleteSubKey(sKeyName + "\\shell\\open\\command");

                //' This deletes a Root entry called "ApplicationName".
                Registry.ClassesRoot.DeleteSubKey(sKeyName + "\\shell\\open");

                //' This deletes a Root entry called "ApplicationName".
                Registry.ClassesRoot.DeleteSubKey(sKeyName + "\\shell");

                //' This deletes a Root entry called "ApplicationName".
                Registry.ClassesRoot.DeleteSubKey(sKeyName);

                //' This deletes the Root entry for the extension to be associated with "ApplicationName".
                sKeyName = "." + extension;
                Registry.ClassesRoot.DeleteSubKey(sKeyName);

                SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }            
        }
                        
        public bool validateAssociation(
            String extension,
            String pathToExecute,
            String applicationName)
        {

            String longPathToExecute;       
            String longPathAssociated;
    
            longPathToExecute = getLongFileName(pathToExecute);
            longPathAssociated = getLongFileName(getAssociatedApp(getTempFile(extension)));
    
            delTempFile (extension);

            if (longPathToExecute != longPathAssociated)
            {
                if (ask(extension, pathToExecute))
                {
                    fAsk f = new fAsk();
                    f.question = m_question.Replace("%1", extension);
                    f.dontAsk = m_dontAsk;
                    f.noButton = m_noButton;
                    f.yesButton = m_yesButton;

                    f.ShowDialog();

                    if (f.result)
                    {
                        associateFileExtension(extension, pathToExecute, applicationName);
                        return true;
                    }
                    else
                    {
                        if (f.dontAskAgain)
                        {
                            saveNotAsk(extension, pathToExecute);
                        }
                        return false;
                    }
                }
                else 
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private bool ask(String extension, String pathToExecute)
        {
            RegistryKey rKey = Registry.CurrentUser.OpenSubKey(C_CROWSOFTKEY_EXTENSIONS);
            String keyVal = (String)rKey.GetValue(extension);
            if (keyVal == null)
                return false;
            else
                return keyVal.ToLower().Contains(pathToExecute.ToLower());
        }

        private void saveNotAsk(
            String extension,
            String pathToExecute)
        {
            RegistryKey rKey = Registry.CurrentUser.OpenSubKey(C_CROWSOFTKEY_EXTENSIONS);
            String keyVal = (String)rKey.GetValue(extension);
            if (keyVal == null)
                keyVal = "";
            rKey.SetValue(extension, keyVal + pathToExecute + "|", RegistryValueKind.String);
        }

        private String getTempFile(String extension)
        {
            String strFile;
            strFile = Path.GetTempPath() + "_Aux_Asoc_." + extension;
            try
            {
                StreamWriter writer = new StreamWriter(strFile);
                writer.Close();
                return strFile;
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        private void delTempFile(String extension)
        {
            String file = getTempFile(extension);
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }    
    }

    #region enum HChangeNotifyEventID
    /// <summary>
    /// Describes the event that has occurred. 
    /// Typically, only one event is specified at a time. 
    /// If more than one event is specified, the values contained 
    /// in the <i>dwItem1</i> and <i>dwItem2</i> 
    /// parameters must be the same, respectively, for all specified events. 
    /// This parameter can be one or more of the following values. 
    /// </summary>
    /// <remarks>
    /// <para><b>Windows NT/2000/XP:</b> <i>dwItem2</i> contains the index 
    /// in the system image list that has changed. 
    /// <i>dwItem1</i> is not used and should be <see langword="null"/>.</para>
    /// <para><b>Windows 95/98:</b> <i>dwItem1</i> contains the index 
    /// in the system image list that has changed. 
    /// <i>dwItem2</i> is not used and should be <see langword="null"/>.</para>
    /// </remarks>
    [Flags]
    enum HChangeNotifyEventID
    {
        /// <summary>
        /// All events have occurred. 
        /// </summary>
        SHCNE_ALLEVENTS = 0x7FFFFFFF,

        /// <summary>
        /// A file type association has changed. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> 
        /// must be specified in the <i>uFlags</i> parameter. 
        /// <i>dwItem1</i> and <i>dwItem2</i> are not used and must be <see langword="null"/>. 
        /// </summary>
        SHCNE_ASSOCCHANGED = 0x08000000,

        /// <summary>
        /// The attributes of an item or folder have changed. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the item or folder that has changed. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
        /// </summary>
        SHCNE_ATTRIBUTES = 0x00000800,

        /// <summary>
        /// A nonfolder item has been created. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the item that was created. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
        /// </summary>
        SHCNE_CREATE = 0x00000002,

        /// <summary>
        /// A nonfolder item has been deleted. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the item that was deleted. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_DELETE = 0x00000004,

        /// <summary>
        /// A drive has been added. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the root of the drive that was added. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_DRIVEADD = 0x00000100,

        /// <summary>
        /// A drive has been added and the Shell should create a new window for the drive. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the root of the drive that was added. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_DRIVEADDGUI = 0x00010000,

        /// <summary>
        /// A drive has been removed. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the root of the drive that was removed.
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_DRIVEREMOVED = 0x00000080,

        /// <summary>
        /// Not currently used. 
        /// </summary>
        SHCNE_EXTENDED_EVENT = 0x04000000,

        /// <summary>
        /// The amount of free space on a drive has changed. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the root of the drive on which the free space changed.
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_FREESPACE = 0x00040000,

        /// <summary>
        /// Storage media has been inserted into a drive. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the root of the drive that contains the new media.
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_MEDIAINSERTED = 0x00000020,

        /// <summary>
        /// Storage media has been removed from a drive. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the root of the drive from which the media was removed. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_MEDIAREMOVED = 0x00000040,

        /// <summary>
        /// A folder has been created. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> 
        /// or <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the folder that was created. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_MKDIR = 0x00000008,

        /// <summary>
        /// A folder on the local computer is being shared via the network. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the folder that is being shared. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_NETSHARE = 0x00000200,

        /// <summary>
        /// A folder on the local computer is no longer being shared via the network. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the folder that is no longer being shared. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_NETUNSHARE = 0x00000400,

        /// <summary>
        /// The name of a folder has changed. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the previous pointer to an item identifier list (PIDL) or name of the folder. 
        /// <i>dwItem2</i> contains the new PIDL or name of the folder. 
        /// </summary>
        SHCNE_RENAMEFOLDER = 0x00020000,

        /// <summary>
        /// The name of a nonfolder item has changed. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the previous PIDL or name of the item. 
        /// <i>dwItem2</i> contains the new PIDL or name of the item. 
        /// </summary>
        SHCNE_RENAMEITEM = 0x00000001,

        /// <summary>
        /// A folder has been removed. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the folder that was removed. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_RMDIR = 0x00000010,

        /// <summary>
        /// The computer has disconnected from a server. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the server from which the computer was disconnected. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// </summary>
        SHCNE_SERVERDISCONNECT = 0x00004000,

        /// <summary>
        /// The contents of an existing folder have changed, 
        /// but the folder still exists and has not been renamed. 
        /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
        /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
        /// <i>dwItem1</i> contains the folder that has changed. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
        /// If a folder has been created, deleted, or renamed, use SHCNE_MKDIR, SHCNE_RMDIR, or 
        /// SHCNE_RENAMEFOLDER, respectively, instead. 
        /// </summary>
        SHCNE_UPDATEDIR = 0x00001000,

        /// <summary>
        /// An image in the system image list has changed. 
        /// <see cref="HChangeNotifyFlags.SHCNF_DWORD"/> must be specified in <i>uFlags</i>. 
        /// </summary>
        SHCNE_UPDATEIMAGE = 0x00008000,

    }
    #endregion // enum HChangeNotifyEventID

    #region public enum HChangeNotifyFlags
    /// <summary>
    /// Flags that indicate the meaning of the <i>dwItem1</i> and <i>dwItem2</i> parameters. 
    /// The uFlags parameter must be one of the following values.
    /// </summary>
    [Flags]
    public enum HChangeNotifyFlags
    {
        /// <summary>
        /// The <i>dwItem1</i> and <i>dwItem2</i> parameters are DWORD values. 
        /// </summary>
        SHCNF_DWORD = 0x0003,
        /// <summary>
        /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of ITEMIDLIST structures that 
        /// represent the item(s) affected by the change. 
        /// Each ITEMIDLIST must be relative to the desktop folder. 
        /// </summary>
        SHCNF_IDLIST = 0x0000,
        /// <summary>
        /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings of 
        /// maximum length MAX_PATH that contain the full path names 
        /// of the items affected by the change. 
        /// </summary>
        SHCNF_PATHA = 0x0001,
        /// <summary>
        /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings of 
        /// maximum length MAX_PATH that contain the full path names 
        /// of the items affected by the change. 
        /// </summary>
        SHCNF_PATHW = 0x0005,
        /// <summary>
        /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings that 
        /// represent the friendly names of the printer(s) affected by the change. 
        /// </summary>
        SHCNF_PRINTERA = 0x0002,
        /// <summary>
        /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings that 
        /// represent the friendly names of the printer(s) affected by the change. 
        /// </summary>
        SHCNF_PRINTERW = 0x0006,
        /// <summary>
        /// The function should not return until the notification 
        /// has been delivered to all affected components. 
        /// As this flag modifies other data-type flags, it cannot by used by itself.
        /// </summary>
        SHCNF_FLUSH = 0x1000,
        /// <summary>
        /// The function should begin delivering notifications to all affected components 
        /// but should return as soon as the notification process has begun. 
        /// As this flag modifies other data-type flags, it cannot by used by itself.
        /// </summary>
        SHCNF_FLUSHNOWAIT = 0x2000
    }
    #endregion // enum HChangeNotifyFlags
}
