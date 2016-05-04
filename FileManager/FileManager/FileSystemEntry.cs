using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;

namespace FileManager
{

    public class FileSystemEntry
    {

        public string Name
        {
            get
            {
                return EntryInfo.Name;
            }
        }

        public string DisplayName
        {
            get; set;
        }

        public string Ext
        {
            get
            {
                if (EntryInfo is FileInfo)
                {
                    var fileInfo = EntryInfo as FileInfo;
                    return fileInfo.Extension.Replace(".", "");
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string Size
        {
            get
            {
                if (EntryInfo is FileInfo)
                {
                    var fileInfo = EntryInfo as FileInfo;
                    return fileInfo.Length.ToString();
                }
                else
                {
                    return "<DIR>";
                }
            }
        }

        public DateTime Date
        {
            get { return EntryInfo.LastWriteTime; }
        }

        public ImageSource Icon
        {
            get;
            set;
        }

        public string Fullpath
        {
            get { return EntryInfo.FullName; }
        }
        

        public FileSystemInfo EntryInfo
        {
            get;
            private set;
        }

        public bool IsDirectory
        {
            get { return EntryInfo is DirectoryInfo; }
        }

        public FileSystemEntry(FileSystemInfo info, ImageSource image)
        {
            EntryInfo = info;
            Icon = image;
            if (IsDirectory)
            {
                DisplayName = EntryInfo.Name;
            }
            DisplayName = Path.GetFileNameWithoutExtension(EntryInfo.FullName);
        }        

        public void Delete()
        {
            if (EntryInfo is DirectoryInfo)
            {
                var info = EntryInfo as DirectoryInfo;
                info.Delete(true);
            }
            else if (EntryInfo is FileInfo)
            {
                var info = EntryInfo as FileInfo;
                info.Delete();
            }
        }

        public void Rename(string newName)
        {
            if (IsDirectory)
            {
                var info = EntryInfo as DirectoryInfo;
                Directory.Move(EntryInfo.FullName, Path.Combine(info.Parent.FullName, newName));
            }
            else
            {
                var info = EntryInfo as FileInfo;
                File.Move(EntryInfo.FullName, Path.Combine(info.DirectoryName, newName));
            }            
        }


    }
}
