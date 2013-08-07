using System;
using System.Collections;
using System.Runtime.Remoting.Channels.Ipc;
using IPCObject;
using System.Windows.Forms;

namespace LiaVM
{
    class ApplicationData
    {
        static ApplicationData instance = null;
        static readonly object padlock = new object();
        public ArrayList consoleContext = new ArrayList();
        public LiaApplicationContext context;
        public IPCRemoteObject ro;
        public IpcServerChannel svr;
        public String VMActivityPath = @"C:\Program Files\Luavis\Lia VM";
        public String VMActivityExcutePath = @"C:\Program Files\Luavis\Lia VM\LiaVMActivity.exe";
        public String VMExcutePath = @"C:\Program Files\Luavis\Lia VM\LiaVM.exe";

        private ArrayList consoleforms = new ArrayList();

        private ApplicationData()
        {

        }

        public void registerConsoleForm(ConsoleForm con)
        {
            consoleforms.Add(con);
        }

        public void removeConsoleForm(ConsoleForm con)
        {
            consoleforms.Remove(con);
        }

        private void reGenConsoleForm()
        {
            for (int i = 0; i < consoleforms.Count; i++)
            {
                ConsoleForm f = (consoleforms[i] as ConsoleForm);

                f.Invoke((MethodInvoker)delegate
                {
                    f.reloadConsole();
                });
            }
        }

        public bool recordConsole(String str)
        {
            consoleContext.Add(str);
            this.reGenConsoleForm();
            return true;
        }

        public static ApplicationData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ApplicationData();
                    }
                    return instance;
                }
            }
        }
    }
}
