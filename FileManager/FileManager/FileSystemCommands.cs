using System.Windows.Input;

namespace FileManager
{
    public static class FileSystemCommands
    {
        public static readonly RoutedUICommand NewFile = new RoutedUICommand
                        (
                                "New File",
                                "New_File",
                                typeof(FileSystemCommands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.F, ModifierKeys.Alt)
                                }
                        );

        public static readonly RoutedUICommand NewFolder = new RoutedUICommand
                        (
                                "New Folder",
                                "New_Folder",
                                typeof(FileSystemCommands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.D, ModifierKeys.Alt)
                                }
                        );

        public static readonly RoutedUICommand Rename = new RoutedUICommand
                        (
                                "Rename",
                                "Rename",
                                typeof(FileSystemCommands),
                                new InputGestureCollection()
                                {
                                        new KeyGesture(Key.R, ModifierKeys.Alt)
                                }
                        );
    }
}
