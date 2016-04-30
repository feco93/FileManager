﻿using System;
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
                    UpdateCopyFromLabel(source);
                    UpdateCopyToLabel(Path.Combine(DestinationPath, sourceFile.Name));
                    var destinationDir = new DirectoryInfo(DestinationPath);
                    await CopyFileAsync(sourceFile, destinationDir, new Progress<int>(percent => FileCopyPrograssBar.Value = percent));
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
                UpdateCopyFromLabel(file.FullName);
                UpdateCopyToLabel(Path.Combine(destDirName, file.Name));
                await CopyFileAsync(file, destinationDir, new Progress<int>(percent => FileCopyPrograssBar.Value = percent));
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

        private void UpdateCopyFromLabel(string source)
        {
            CopyFrom.Content = string.Format("Copy from: {0}", source);
        }

        private void UpdateCopyToLabel(string destination)
        {
            CopyTo.Content = string.Format("Copy to: {0}", destination);
        }

        private async Task CopyFileAsync(FileInfo sourceFile, DirectoryInfo destinationDir, IProgress<int> progress)
        {
            using (FileStream SourceStream = File.Open(sourceFile.FullName, FileMode.Open))
            {
                using (FileStream DestinationStream = File.Create(Path.Combine(destinationDir.FullName, sourceFile.Name)))
                {
                    int bufferSize = 4096;
                    int totalRead = 0;
                    while (true)
                    {
                        byte[] result = new byte[bufferSize];
                        int readedBytes = await SourceStream.ReadAsync(result, 0, bufferSize);
                        if (readedBytes == 0)
                            break;                        
                        await DestinationStream.WriteAsync(result, 0, readedBytes);
                        totalRead += readedBytes;
                        if (progress != null)
                        {
                            progress.Report((int)(totalRead * 100 / SourceStream.Length));
                        }                        
                    }
                }
            }
        }
    }
}
