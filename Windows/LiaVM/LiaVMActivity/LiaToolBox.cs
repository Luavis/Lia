using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IPCObject;
using System.Collections;
using Lia;

namespace LiaVMActivity
{
    class LiaToolBox
    {
        public LiaInterpreter li = new LiaInterpreter("WIN");

        private String code = "";

        public String Code
        {
            get
            {
                return code;
            }
        }

        public String programName = "";

        public String ProgramName
        {
            get
            {
                return programName;
            }
        }

        IpcClientChannel clientChannel;
        IPCRemoteObject obj;

        [SecurityPermission(SecurityAction.Demand)]
        public LiaToolBox(String path)
        {
            try
            {
                clientChannel = new IpcClientChannel();
                ChannelServices.RegisterChannel(clientChannel, false);
                RemotingConfiguration.RegisterWellKnownClientType(typeof(IPCRemoteObject), "ipc://LiaVM/lia");
                obj = new IPCRemoteObject();
            }
            catch (Exception e)
            {
                if (e != null)
                {
                    MessageBox.Show("VM이 종료되어 있습니다.");
                    Environment.Exit(2);
                }
            }

            String programWithExt = Path.GetFileName(path);
            if (Path.GetExtension(programWithExt) == "li")
            {
                MessageBox.Show("정상적인 파일이 아닙니다.");
                Environment.Exit(2);
            }

            this.programName = Path.GetFileNameWithoutExtension(programWithExt);

            setName();
            readCode(path);

            LiaConsoleWriteLine("Application is Launch");
        }

        System.Threading.Timer WindowCounterTimer;

        public void applicationDeadCountStart()
        {
            WindowCounterTimer = new System.Threading.Timer(new TimerCallback(this.WindowCount), this,1000,250);
            //System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            //timer.Interval = 100;
            //timer.Tick += new EventHandler(WindowCount);
            //timer.Start();
        }

        public void WindowCount(object state)
        {
            int windowList = ApplicationData.Instance.CountRegistedForm();

            if (windowList <= 0)
            {
                WindowCounterTimer.Dispose();
                Application.ExitThread();
                Application.Exit();
            }
        }

        public void LiaConsoleWriteLine(String context)
        {
            String date = DateTime.Now.ToString("F");
            try
            {
                obj.getConsoleString(date + " [" + programName + "] " + ": " + context);
            }
            catch
            {
                MessageBox.Show("VM과의 연결이 끊겼습니다.");
                Environment.Exit(2);
            }
        }

        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);

        private void readCode(String path)
        {
            const int bufferSize = 1024;

            var sb = new StringBuilder();
            var buffer = new Char[bufferSize];
            var length = 0L;
            var totalRead = 0L;
            var count = bufferSize;

            using (var sr = new StreamReader(path))
            {
                length = sr.BaseStream.Length;
                while (count > 0)
                {
                    count = sr.Read(buffer, 0, bufferSize);
                    sb.Append(buffer, 0, count);
                    totalRead += count;
                }
            }

            this.code = sb.ToString();
        }
        private void setName()
        {
            Process p = Process.GetCurrentProcess();
            Thread.Sleep(100);  // <-- ugly hack
            SetWindowText(p.MainWindowHandle, this.programName);
        }

    }
}
