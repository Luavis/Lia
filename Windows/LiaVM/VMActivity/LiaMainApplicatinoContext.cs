using System;
using System.Collections.Generic;
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
using System.Reflection;


namespace VMActivity
{
    class LiaToolBox
    {
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

            LiaConsoleWriteLine("Applicatino is Launch");
        }

        public void LiaConsoleWriteLine(String context)
        {
            String date = DateTime.Now.ToString("F");
            obj.getConsoleString(date + " [" + programName + "] " + ": " + context);
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

    class LiaMainApplicatinoContext : ApplicationContext
    {

        LiaToolBox box;

        static Dictionary<int, Object> dynamicTable = new Dictionary<int, Object>();

        public void print(LiaInterpreterVar v)
        {
            string str = v.getParameter("str").getString();
            box.LiaConsoleWriteLine(str);
        }

        public void dyCall(LiaInterpreterVar v)
        {
            Object called = dynamicTable[v.getParameter("object").getInt()];

            Type calledType = called.GetType();

            ArrayList param = new ArrayList();

            int len = v.getParameter("param").getArrayLength();

            for (int i = 0; i < len; i++)
            {
                param.Add(v.getParameter("param").getArrayIndex(i));
            }

            String s = (String)calledType.InvokeMember(
                            v.getParameter("function").getString(),
                            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                            null,
                            called,
                           param.ToArray());
        }

        public void dyGen(LiaInterpreterVar v)
        {
            LiaInterpreterVar oV = v.getParameter("class");
            Type t = Type.GetType(oV.getString());

            Object obj = null;

            try
            {
                obj = Activator.CreateInstance(t);
            }
            catch (ArgumentException)
            {
                box.LiaConsoleWriteLine("Dynamic type is not exist");
                return;
            }

            if (obj == null)
            {

                box.LiaConsoleWriteLine("Dynamic Object Got NULL");
                return;
            }

            v.getReturnVar().setInt(obj.GetHashCode());
            dynamicTable.Add(obj.GetHashCode(), obj);
        }

        static public void add(LiaInterpreterVar v)
        {
            v.getParameter("i").setInt(123);
        }

        public LiaMainApplicatinoContext(String path)
        {
            box = new LiaToolBox(path);
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            LiaInterpreter li = new LiaInterpreter("WIN");

            li.addNative("@func dyGen(class)", new LiaInterpreter.CSCallBack(dyGen));
            li.addNative("@func dyCall(object,function,param)", (new LiaInterpreter.CSCallBack(dyCall)));
            li.addNative("@func print(str)", new LiaInterpreter.CSCallBack(print));

            string ret = li.call(box.Code);
            Application.ExitThread();
        }

        public void OnApplicationExit(object sender, EventArgs e)
        {
            box.LiaConsoleWriteLine("Application is Exit");
        }
    }
}
