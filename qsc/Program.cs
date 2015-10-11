using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using qqq;
using System.Text.RegularExpressions;
using System.IO;
namespace qsc
{
    class Program
    {
        static string _sym = "PWT.TO";
        private static string get_default_file()
        {
            qqq_file_util utils = new qqq_file_util();
            utils.create_if_path_not_exist();
            string fname = utils.build();
            if (utils.check_file_exists() == false)
            {
                StockFetch obj = new StockFetch();
                obj.AwkMode = true;
                obj.OutputFileName = fname;
                obj.DownloadData(_sym);
            }
            return utils.build();

        }
        static private string get_tmp_file()
        {
            qqq_file_util utils = new qqq_file_util();
            utils.create_if_path_not_exist();
            return utils.build("tmp");


        }
        static private string get_stockname_file()
        {
            string spath = Environment.CurrentDirectory;
            
            return Path.Combine(spath, "stockname");
        }

        static void Main(string[] args)
        {
            string result = "OK";
            if (args.Length == 0) {
                result = "ERROR";
                goto main_exit;
            }
            string sym = args[0];

            string spath = Environment.CurrentDirectory;
            string sfilename = "qqq.csv";
            StockFetch obj = new StockFetch();
            obj.AwkMode = true;
            obj.OutputFileName = Path.Combine(spath, sfilename);
            
            {
                // write stockname
                string stockname_fname = get_stockname_file();

                using (System.IO.StreamWriter file =
                      new System.IO.StreamWriter(stockname_fname))
                {
                    file.WriteLine(sym);
                }
            }

            Match e = Regex.Match(sym, ".TO");
            if (e.Success)
            {
                if (obj.DownloadData(sym) == false)
                {
                    result = "ERROR";
                    goto main_exit;
                }
               
            }
            else
            {
                if (obj.DownloadData2(sym, get_tmp_file()) == false)
                {
                    result = "ERROR";
                    goto main_exit;
                }
               
            }

        main_exit:
            Console.WriteLine(result);
        }
    }


    public class UtilityArgs
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }
    }


}
