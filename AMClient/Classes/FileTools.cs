using Astral.Controllers;
using System;
using System.IO;
using System.Windows.Forms;

namespace ACTP0Tools.Classes
{
    public static class FileTools
    {
        private static OpenFileDialog openDialog;
        private static SaveFileDialog saveDialog;

        public static OpenFileDialog GetOpenDialog(string fileName = "", string initialDir = "", string filter = "", string defaultExtension = "")
        {
            if (openDialog is null)
            {
                openDialog = new OpenFileDialog();
                openDialog.Disposed += Dialog_Disposed;
            }
            openDialog.Filter = string.IsNullOrEmpty(filter)
                              ? "All files (*.*)| *.*"
                              : filter;
            if (string.IsNullOrEmpty(initialDir))
            {
                if (!string.IsNullOrEmpty(fileName))
                    initialDir = Path.GetDirectoryName(fileName);
                if (string.IsNullOrEmpty(initialDir))
                    initialDir = Directories.ProfilesPath;
            }
            openDialog.InitialDirectory = initialDir;

            if (string.IsNullOrEmpty(defaultExtension))
            {
                defaultExtension = Path.GetExtension(fileName);
            }
            openDialog.DefaultExt = defaultExtension;
            openDialog.FileName = string.IsNullOrEmpty(fileName)
                                ? string.Empty
                                : Path.GetFileNameWithoutExtension(fileName);
            openDialog.CheckFileExists = false;

            return openDialog;
        }
        public static SaveFileDialog GetSaveDialog(string fileName = "", string initialDir = "", string filter = "", string defaultExtension = "")
        {
            if (saveDialog is null)
            {
                saveDialog = new SaveFileDialog();
                saveDialog.Disposed += Dialog_Disposed;
            }
            saveDialog.Filter = string.IsNullOrEmpty(filter)
                ? "All files (*.*)| *.*"
                : filter;
            if (string.IsNullOrEmpty(initialDir))
            {
                if(!string.IsNullOrEmpty(fileName))
                    initialDir = Path.GetDirectoryName(fileName);
                if (string.IsNullOrEmpty(initialDir))
                    initialDir = Directories.ProfilesPath;
            }
            saveDialog.InitialDirectory = initialDir;

            if (string.IsNullOrEmpty(defaultExtension))
            {
                defaultExtension = Path.GetExtension(fileName);
            }
            saveDialog.DefaultExt = defaultExtension;
            saveDialog.FileName = string.IsNullOrEmpty(fileName) 
                                ? string.Empty
                                : Path.GetFileNameWithoutExtension(fileName);
            saveDialog.CheckFileExists = false;

            return saveDialog;
        }
        private static void Dialog_Disposed(object sender, EventArgs e)
        {
            switch (sender)
            {
                case OpenFileDialog _:
                    openDialog.Disposed -= Dialog_Disposed;
                    openDialog = null;
                    break;
                case SaveFileDialog _:
                    saveDialog.Disposed -= Dialog_Disposed;
                    saveDialog = null;
                    break;
            }
        }

        /// See here for the following 2 methods: <see href="http://stackoverflow.com/questions/703281/getting-path-relative-to-the-current-working-directory"/>
        /// <summary>
        /// Gets the relative path ("..\..\to_file_or_dir") of the current file/dir (to) in relation to another (from)
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo from)
        {
            return from.GetRelativePathTo(to);
        }

        public static string GetRelativePathFrom(this string to, string from)
        {
            if (string.IsNullOrEmpty(from))
                return to;
            return from.GetRelativePathTo(to);
        }

        /// <summary>
        /// Gets the relative path ("..\..\to_file_or_dir") of another file or directory (to) in relation to the current file/dir (from)
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static string GetRelativePathTo(this FileSystemInfo from, FileSystemInfo to)
        {
            string GetPath(FileSystemInfo fsi)
            {
                var d = fsi as DirectoryInfo;
                return d == null ? fsi.FullName : d.FullName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            }

            var fromPath = GetPath(from);
            var toPath = GetPath(to);

            return GetRelativePathTo(fromPath, toPath);
        }

        public static string GetRelativePathTo(this string from, string to)
        {
            if (string.IsNullOrEmpty(from))
                return to;
            if (string.IsNullOrEmpty(to))
                return string.Empty;

            var fromPath = Path.GetFullPath(from);
            var toPath = Path.GetFullPath(to);

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
