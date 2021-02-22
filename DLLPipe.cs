using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace urnamespace
{
    class DLLPipe
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        public static bool NamedPipeExist(string pipeName)
        {
            bool result;
            try
            {
                bool flag = !DLLPipe.WaitNamedPipe("\\\\.\\pipe\\" + pipeName, 0);
                if (flag)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    bool flag2 = lastWin32Error == 0;
                    if (flag2)
                    {
                        return false;
                    }
                    bool flag3 = lastWin32Error == 2;
                    if (flag3)
                    {
                        return false;
                    }
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static void Execute(string script)
        {

            bool flag = DLLPipe.NamedPipeExist(DLLPipe.luapipename);
            if (flag)
            {
                new Thread(delegate ()
                {
                    try
                    {
                        using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", DLLPipe.luapipename, PipeDirection.Out))
                        {
                            namedPipeClientStream.Connect();
                            using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream, Encoding.Default, 999999))
                            {
                                streamWriter.Write(script);
                                streamWriter.Dispose();
                            }
                            namedPipeClientStream.Dispose();
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Error while connecting to pipe")
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
                    }
                }).Start();
            }
            else
            {
              
                
               MessageBox.Show("Please Inject for execute a script!");
            }
        }

        public static string luapipename = "pipename";



        public static void Inject()
        {
            DLLInject.DllInjectionResult dllInjectionResult = DLLInject.DllInjector.GetInstance.Inject("RobloxPlayerBeta", Application.StartupPath + "//exploit-dll.dll");
            if (dllInjectionResult == DLLInject.DllInjectionResult.Success)
            {
                return;
            }
            switch (dllInjectionResult)
            {
              
                case DLLInject.DllInjectionResult.DllNotFound:
                  MessageBox.Show("dll not found")
                    return;
                case DLLInject.DllInjectionResult.GameProcessNotFound:
                  MessageBox.Show("Roblox not found please open roblox");
                    return;
                case DLLInject.DllInjectionResult.InjectionFailed:
 MessageBox.Show("Failed while injecting the dll");
                    return;
               
                    
                default:
                    return;
            }
        }


    }
}

