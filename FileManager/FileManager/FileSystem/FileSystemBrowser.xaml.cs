using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for FileSystemBrowser.xaml
    /// </summary>
    public partial class FileSystemBrowser : UserControl
    {

        private FileSystemWatcher watcher;

        private string _actualPath;
        public string ActualPath
        {
            get
            {
                return _actualPath;
            }
            private set
            {
                _actualPath = value;

                fileSystemView.DataContext = GetEntries(ActualPath);
                watcher.Path = ActualPath;
                watcher.EnableRaisingEvents = true;
            }
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            await fileSystemView.Dispatcher.InvokeAsync(() =>
            {
                fileSystemView.DataContext = GetEntries(ActualPath);
            });
        }

        private async void OnRenamed(object source, RenamedEventArgs e)
        {
            await fileSystemView.Dispatcher.InvokeAsync(() =>
            {
                fileSystemView.DataContext = GetEntries(ActualPath);
            });
        }

        public FileSystemBrowser()
        {
            watcher = new FileSystemWatcher();
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            InitializeComponent();
            discCb.ItemsSource = Directory.GetLogicalDrives();
            discCb.SelectedIndex = 0;
        }

        private void discCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newPath = e.AddedItems.Count > 0 ? e.AddedItems[0].ToString() : string.Empty;
            var oldPath = e.RemovedItems.Count > 0 ? e.RemovedItems[0].ToString() : string.Empty;
            try
            {
                ActualPath = newPath;
                fileSystemView.Focus();
            }
            catch (IOException exc)
            {
                var dialog = new DriveChooserDialog(exc.Message);
                dialog.Owner = Window.GetWindow(this);
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dialog.driveCb.SelectedIndex = discCb.SelectedIndex;
                bool? dialogResult = dialog.ShowDialog();
                while (dialogResult == true && discCb.SelectedItem.Equals(dialog.driveCb.SelectedItem.ToString()))
                {
                    dialog = new DriveChooserDialog(exc.Message);
                    dialog.driveCb.SelectedIndex = discCb.SelectedIndex;
                    dialogResult = dialog.ShowDialog();
                }
                if (dialogResult == true)
                {
                    discCb.SelectedItem = dialog.driveCb.SelectedItem.ToString();
                }
                else
                {
                    discCb.SelectedItem = oldPath;
                }
            }
        }

        List<FileSystemEntry> GetEntries(string path)
        {
            List<FileSystemEntry> entries = new List<FileSystemEntry>();
            System.Drawing.Bitmap bmp;
            BitmapSource source;
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Parent != null)
            {
                bmp = Properties.Resources.LeftUp;
                source = Imaging.CreateBitmapSourceFromHBitmap(
                           bmp.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
                FileSystemEntry d = new FileSystemEntry(directoryInfo.Parent, source, false);
                d.DisplayName = "[..]";
                entries.Add(d);
            }
            var directories = directoryInfo.GetDirectories();
            var filteredDirectories = directories.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));
            bmp = Properties.Resources.Folder;
            source = Imaging.CreateBitmapSourceFromHBitmap(
                       bmp.GetHbitmap(),
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       BitmapSizeOptions.FromEmptyOptions());
            foreach (var dirInfo in filteredDirectories)
            {
                FileSystemEntry d = new FileSystemEntry(dirInfo, source);
                entries.Add(d);
            }
            FileInfo[] files = directoryInfo.GetFiles();
            var filteredFiles = files.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden));
            var buffer = new Dictionary<string, BitmapSource>();
            foreach (var fileInfo in filteredFiles)
            {
                if (!buffer.ContainsKey(fileInfo.Extension))
                {
                    var icon = System.Drawing.Icon.ExtractAssociatedIcon(fileInfo.FullName);
                    source = Imaging.CreateBitmapSourceFromHBitmap(
                               icon.ToBitmap().GetHbitmap(),
                               IntPtr.Zero,
                               Int32Rect.Empty,
                               BitmapSizeOptions.FromEmptyOptions());
                    buffer.Add(fileInfo.Extension, source);
                }
                else
                {
                    source = buffer[fileInfo.Extension];
                }
                FileSystemEntry d = new FileSystemEntry(fileInfo, source);
                entries.Add(d);
            }
            return entries;
        }

        void listViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = e.Source as ListViewItem;
            FileSystemEntry entry = item.DataContext as FileSystemEntry;
            if (entry == null)
                return;
            OnItemSelected(entry);
        }

        private void OnItemSelected(FileSystemEntry entry)
        {
            if (entry.IsDirectory)
            {
                try
                {
                    ActualPath = entry.Fullpath;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error");
                }

            }
            else
            {
                Process.Start(entry.Fullpath);
            }
        }

        private void fileSystemView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            fileSystemView.Focus();
        }

        private void fileSystemView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //Point mpos = e.GetPosition(null);
            //Vector diff = start - mpos;

            //if (e.LeftButton == MouseButtonState.Pressed &&
            //    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance * 2 ||
            //    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance * 2))
            //{
            //    var SelectedItem = fileSystemView.SelectedItem as FileSystemEntry;
            //    if (fileSystemView.SelectedItems.Count == 0 || SelectedItem.DisplayName.Equals("[..]"))
            //        return;

            //    string[] files = new string[fileSystemView.SelectedItems.Count];
            //    int ix = 0;
            //    foreach (FileSystemEntry nextSel in fileSystemView.SelectedItems)
            //    {
            //        files[ix] = nextSel.Fullpath;
            //        ++ix;
            //    }
            //    string dataFormat = DataFormats.FileDrop;
            //    DataObject dataObject = new DataObject(dataFormat, files);
            //    DragDrop.DoDragDrop(fileSystemView, dataObject, DragDropEffects.Copy);
            //}
        }

        private void fileSystemView_Drop(object sender, DragEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                string.Format("Copy file(s) to {0}", ActualPath),
                "Copy Confirmation",
                MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string[] data = e.Data.GetData(DataFormats.FileDrop) as string[];
                var copyDialog = new CopyDialog(ActualPath, data);
                copyDialog.Show();
            }
        }

        private void fileSystemView_KeyDown(object sender, KeyEventArgs e)
        {
            var selectedEntry = fileSystemView.SelectedItem as FileSystemEntry;
            switch (e.Key)
            {
                case Key.Enter:
                    if (selectedEntry == null)
                        return;
                    OnItemSelected(selectedEntry);
                    break;
            }
        }

        private void DeleteSelectedFiles()
        {
            if (fileSystemView.SelectedItems.Count == 0)
                return;
            MessageBoxResult messageBoxResult = MessageBox.Show(
                            "Are you sure delete selected file(s)?",
                            "Delete Confirmation",
                            MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (FileSystemEntry entry in fileSystemView.SelectedItems)
                {
                    try
                    {
                        entry.Delete();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error");
                    }
                }
            }
        }

        private void ParentPathBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var info = new DirectoryInfo(ActualPath);
            if (info.Parent == null)
                return;
            ActualPath = info.Parent.FullName;
        }

        private void RootPathBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var info = new DirectoryInfo(ActualPath);
            ActualPath = info.Root.ToString();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = e.Command as RoutedUICommand;
            if (command == ApplicationCommands.Delete)
            {
                DeleteSelectedFiles();
            }
            if (command == ApplicationCommands.Copy || command == ApplicationCommands.Cut)
            {
                var files = new StringCollection();
                DataObject data = new DataObject();
                foreach (FileSystemEntry entry in fileSystemView.SelectedItems)
                {
                    if (!entry.IsEditable)
                        continue;
                    files.Add(entry.Fullpath);
                }

                if (files.Count == 0)
                    return;

                data.SetFileDropList(files);
                var dragDropEffect = command == ApplicationCommands.Copy ? DragDropEffects.Copy : DragDropEffects.Move;
                data.SetData("Preferred DropEffect", dragDropEffect);

                Clipboard.Clear();
                Clipboard.SetDataObject(data, true);
            }
            if (command == ApplicationCommands.Paste)
            {
                if (!Clipboard.ContainsFileDropList())
                {
                    return;
                }
                string[] sources = new string[Clipboard.GetFileDropList().Count];
                Clipboard.GetFileDropList().CopyTo(sources, 0);
                var dialog = new CopyDialog(ActualPath, sources);
                try
                {
                    dialog.ShowDialog();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error");
                }
            }
            if (command == FileSystemCommands.NewFolder)
            {
                CreateNewFolder();
            }
            if (command == FileSystemCommands.NewFile)
            {
                CreateNewFile();
            }
            if (command == FileSystemCommands.Rename)
            {
                var SelectedItem = fileSystemView.SelectedItem as FileSystemEntry;
                if (SelectedItem == null || !SelectedItem.IsEditable)
                {
                    return;
                }
                var dialog = new RenameDialog("New name:", SelectedItem.Name);
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == true)
                {
                    SelectedItem.Rename(dialog.Input.Text);
                }

            }

        }

        void CmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var SelectedItem = fileSystemView.SelectedItem as FileSystemEntry;
            if (SelectedItem.IsEditable)
                e.CanExecute = true;
        }

        private void CreateNewFile()
        {
            var dialog = new RenameDialog("New file:");
            dialog.Owner = Window.GetWindow(this);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var dialogResult = dialog.ShowDialog();
            if (dialogResult == true)
            {
                try
                {
                    using (var fileStream = File.Create(Path.Combine(ActualPath, dialog.Input.Text)))
                    {
                    }
                }                
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error");
                }
            }
        }

        private void CreateNewFolder()
        {
            var dialog = new RenameDialog("New folder:");
            dialog.Owner = Window.GetWindow(this);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var dialogResult = dialog.ShowDialog();
            if (dialogResult == true)
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(ActualPath, dialog.Input.Text));
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error");
                }
            }
        }
    }

}
