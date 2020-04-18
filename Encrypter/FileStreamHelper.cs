using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

/// <summary>
/// https://www.cyberforum.ru/post9373739.html
/// </summary>
namespace Encrypter
{
    public static class FileStreamHelper
    {
        public static FileStream OpenWithStream(string fileName, string streamName, FileMode mode, FileAccess access)
        {
            return OpenWithStream(fileName, streamName, mode, access, FileShare.None);
        }

        public static FileStream OpenWithStream(string fileName, string streamName, FileMode mode, FileAccess access, FileShare share)
        {
            uint desiredAccess = (access & FileAccess.Read) == FileAccess.Read ? GenericRead : 0;
            desiredAccess |= (access & FileAccess.Write) == FileAccess.Write ? GenericWrite : 0;

            SafeFileHandle fileHandle = CreateFile(
                fileName + ":" + streamName,
                desiredAccess,
                share,
                IntPtr.Zero,
                mode,
                FileAttributes.Normal,
                IntPtr.Zero
            );
            if (fileHandle.IsInvalid)
            {
                int win32error = Marshal.GetLastWin32Error();
                Marshal.ThrowExceptionForHR(win32error);
                switch (win32error)
                {
                    case 2:
                        throw new FileNotFoundException("Failed to open file with specified stream", fileName + ":" + streamName);
                    default:
                        throw new IOException("Failed to open file with specified stream");
                }
            }
            return new FileStream(fileHandle, access);
        }

        const uint GenericRead = 0x80000000;
        const uint GenericWrite = 0x40000000;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            uint desiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr hTemplateFile
        );
    }
}