using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Holly_X
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
                        HollyMsgBoxClass Msg = new HollyMsgBoxClass();
                        Msg.Error("0x7950002", "Error while connecting to pipe");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
                    }
                }).Start();
            }
            else
            {
              
                HollyMsgBoxClass Msg = new HollyMsgBoxClass();
                Msg.Error("Not injected (0x01)", "Please Inject for execute a script!");
            }
        }

        public static string luapipename = "furkisgay";



        public static void Inject()
        {
            DLLInject.DllInjectionResult dllInjectionResult = DLLInject.DllInjector.GetInstance.Inject("RobloxPlayerBeta", Application.StartupPath + "//Holly-main.dll");
            if (dllInjectionResult == DLLInject.DllInjectionResult.Success)
            {
                return;
            }
            switch (dllInjectionResult)
            {
              
                case DLLInject.DllInjectionResult.DllNotFound:
                    HollyMsgBoxClass Msg = new HollyMsgBoxClass();
                    Msg.Error("Dll not found (0x02)", "<Holly-main.dll> not found please reinstall Holly X");
                    return;
                case DLLInject.DllInjectionResult.GameProcessNotFound:
                    HollyMsgBoxClass Msg2 = new HollyMsgBoxClass();
                    Msg2.Error("Roblox not found (0x034)", "Roblox not found please open roblox");
                    return;
                case DLLInject.DllInjectionResult.InjectionFailed:

                    HollyMsgBoxClass Msg3 = new HollyMsgBoxClass();
                    Msg3.Error("Failed (0x0345201)", "Failed while injecting the dll");
                    return;
               
                    
                default:
                    return;
            }
        }


    }
}

