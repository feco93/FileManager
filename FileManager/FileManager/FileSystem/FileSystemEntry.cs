using System;
using System.ComponentModel;
using System.Globalization;
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

        public string Date
        {
            get { return EntryInfo.LastWriteTime.ToString("yyyy.MM.dd HH:mm"); }
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

        public bool IsEditable { get; private set; }

        public FileSystemEntry(FileSystemInfo info, ImageSource image, bool editable = true)
        {
            EntryInfo = info;
            Icon = image;
            if (IsDirectory)
            {
                DisplayName = EntryInfo.Name;
            }
            else
            {
                DisplayName = Path.GetFileNameWithoutExtension(EntryInfo.FullName);
            }            
            IsEditable = editable;
        }        

        public void Delete()
        {
            if (!IsEditable)
            {
                throw new InvalidOperationException("The file system entry is not editable");
            }
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
            if (!IsEditable)
            {
                throw new InvalidOperationException("The file system entry is not editable");
            }
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
