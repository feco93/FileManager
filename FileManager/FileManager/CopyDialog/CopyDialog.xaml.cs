using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for CopyDialog.xaml
    /// </summary>
    public partial class CopyDialog : Window
    {
        public ICollection<string> Sources;

        public string DestinationPath;

        public CopyDialog(string destinationPath, params string[] sources)
        {
            DestinationPath = destinationPath;
            Sources = sources;
            InitializeComponent();
            Loaded += CopyDialog_Loaded;
        }

        private async void CopyDialog_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var source in Sources)
            {
                FileAttributes attr = File.GetAttributes(source);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var sourceDir = new DirectoryInfo(source);
                    await CopyDirectoryAsync(sourceDir.FullName, Path.Combine(DestinationPath, sourceDir.Name), true);
                }
                else
                {
                    var sourceFile = new FileInfo(source);
                    var destinationDir = new DirectoryInfo(DestinationPath);
                    await CopyFileAsync(sourceFile, destinationDir);
                }
            }
            Close();
        }

        private async Task CopyDirectoryAsync(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new DirectoryInfo(sourceDirName);
            var destinationDir = new DirectoryInfo(destDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destinationDir.FullName))
            {
                Directory.CreateDirectory(destinationDir.FullName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                await CopyFileAsync(file, destinationDir);
            }
            
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    await CopyDirectoryAsync(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private async Task CopyFileAsync(FileInfo source, DirectoryInfo destinationDir)
        {
            try
            {
                UpdateCopyFromLabel(source.FullName);
                UpdateCopyToLabel(Path.Combine(destinationDir.FullName, source.Name));
                await FileSystemMethods.CopyFileAsync(source, destinationDir,
                    new Progress<int>(percent => FileCopyPrograssBar.Value = percent));
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error");
            }            
        }

        private void UpdateCopyFromLabel(string source)
        {
            CopyFrom.Content = string.Format("Copy from: {0}", source);
        }

        private void UpdateCopyToLabel(string destination)
        {
            CopyTo.Content = string.Format("Copy to: {0}", destination);
        }
        
    }
}
