using System;
using System.Collections;
using System.Windows.Forms;
using Lia;
using System.Threading;


namespace LiaVMActivity
{
    class LiaMainApplicationContext : ApplicationContext
    {
        public LiaMainApplicationContext(String path)
        {
            ApplicationData.Instance.box = new LiaToolBox(path);
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            LiaInterpreter li = ApplicationData.Instance.box.li;

            li.addNative("@func dyGen(class)", new LiaInterpreter.CSCallBack(LiaDefaultFunction.dyGen));
            li.addNative("@func dyCall(object,function,param)", (new LiaInterpreter.CSCallBack(LiaDefaultFunction.dyCall)));
            li.addNative("@func print(str)", new LiaInterpreter.CSCallBack(LiaDefaultFunction.print));
            li.addNative("@func parseInt(str)", new LiaInterpreter.CSCallBack(LiaDefaultFunction.parseInt));
            li.addNative("@func parseFloat(str)", new LiaInterpreter.CSCallBack(LiaDefaultFunction.parseFloat));
            
            //this.ApplicationFunctionStart();
            Thread th = new Thread(new ThreadStart(this.ApplicationFunctionStart));
        }

        public void ApplicationFunctionStart()
        {
            string ret = ApplicationData.Instance.box.li.call(ApplicationData.Instance.box.Code);
            ApplicationData.Instance.box.LiaConsoleWriteLine(ret);
            ApplicationData.Instance.box.applicationDeadCountStart();
        }

        public void OnApplicationExit(object sender, EventArgs e)
        {
            ApplicationData.Instance.box.LiaConsoleWriteLine("Application is Exit");
            Environment.Exit(0);
        }
    }
}
