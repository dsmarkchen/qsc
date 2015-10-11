using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Text.RegularExpressions;

namespace qqq
{
    public class qqq_file_util
    {

        #region constructor
        public qqq_file_util()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            spath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), _jobwork_path);

        }
        #endregion

        #region methods
        public string build(string filename_ = null)
        {
            string fname;
            if (filename_ == null || filename_ == "")
                fname = Path.Combine(spath, _file_name);
            else
                fname = Path.Combine(spath, filename_);

            return fname;
        }

        public string build_fname(string fn)
        {
            return Path.Combine(spath, fn);
        }

        public void create_if_path_not_exist()
        {
            if (!Directory.Exists(spath))
            {
                Directory.CreateDirectory(spath);
            }

        }
        public bool check_file_exists()
        {
            string fname = build();
            return File.Exists(fname);
        }
        #endregion

        #region variables
        string _file_name = "qqq.csv";
        string _jobwork_path = @"JobWork\Logs";
        string spath;
        #endregion
    }


    
    public static class AppConsoleExt
    {

        [DllImport("kernel32.dll",
               EntryPoint = "GetStdHandle",
               SetLastError = true,
               CharSet = CharSet.Auto,
               CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]

        private static extern int AllocConsole();
        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;

        static public void init()
        {
#if DEBUG
            Console.WriteLine("This text you can see in debug output window.");
#endif
            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
#if DEBUG
            Console.WriteLine("This text you can see in console window.");
#endif
        }
    }
}
