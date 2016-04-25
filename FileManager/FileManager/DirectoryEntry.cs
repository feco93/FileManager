using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FileManager
{
    public enum EntryType
    {
        Dir,
        File
    }

    public class DirectoryEntry
    {
        private string _name;
        private string _fullpath;
        private string _lastpath;
        private string _ext;
        private string _size;
        private DateTime _date;
        private ImageSource _icon;
        private EntryType _type;

        public DirectoryEntry(string name, string fullname, string ext, string size, DateTime date, ImageSource image, EntryType type)
        {
            _name = name;
            _fullpath = fullname;
            _ext = ext;
            _size = size;
            _date = date;
            _icon = image;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Ext
        {
            get { return _ext; }
            set { _ext = value; }
        }

        public string Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public ImageSource Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
            }
        }

        public EntryType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Fullpath
        {
            get { return _fullpath; }
            set { _fullpath = value; }
        }

        public string Lastpath
        {
            get { return _lastpath; }
            set { _lastpath = value; }
        }
    }
}
