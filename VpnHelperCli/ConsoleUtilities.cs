using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VpnHelperCli
{
    public static class ConsoleUtilities
    {
        private const int STD_INPUT_HANDLE = -10;

        public static void CancelReadLine()
        {
            //https://stackoverflow.com/a/58475263/221018
            var handle = GetStdHandle(STD_INPUT_HANDLE);
            CancelIoEx(handle, IntPtr.Zero);
        }

        public static string ReadLine(string cancelMessage)
        {
            try
            {
                return Console.ReadLine();
            }
            // Handle the exception when the operation is canceled
            catch (InvalidOperationException)
            {
                Console.WriteLine(cancelMessage);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(cancelMessage);
            }

            return string.Empty;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
    }
}