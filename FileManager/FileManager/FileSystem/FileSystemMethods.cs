using System;
using System.IO;
using System.Threading.Tasks;

namespace FileManager
{
    static class FileSystemMethods
    {

        public static async Task CopyFileAsync(FileInfo sourceFile, DirectoryInfo destinationDir, IProgress<int> progress)
        {
            using (FileStream SourceStream = File.Open(sourceFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
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
