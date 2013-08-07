using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Permissions;
using System.Windows.Forms;
using IPCObject;
using Microsoft.Win32;

namespace LiaVM
{
    static class Program
    {

        static void setRegister()
        {
        }

        static void openIPCServer()
        {
            IpcServerChannel svr = new IpcServerChannel("LiaVM");
            ChannelServices.RegisterChannel(svr, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(IPCRemoteObject), "lia", WellKnownObjectMode.Singleton);

            Console.WriteLine("Listening on " + svr.GetChannelUri());
            IPCRemoteObject ro = new IPCRemoteObject(new IPCCaller(ApplicationData.Instance.recordConsole));

            ApplicationData.Instance.svr = svr;
            ApplicationData.Instance.ro = ro;
        }

        [STAThread]
        [SecurityPermission(SecurityAction.Demand)]
        static void Main(String[] arg)
        {
            try
            {
                setRegister();
                try
                {
                    openIPCServer();
                    ApplicationData.Instance.context = new LiaApplicationContext();
                    Application.Run(ApplicationData.Instance.context);
                }
                catch (Exception e)
                {
                    if (e != null)
                    {
                        MessageBox.Show("이미 실행중 입니다.");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("알 수 없는 에러가 발생하였습니다.");
            }
        }

    }
}
