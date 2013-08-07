using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lia;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;

namespace LiaVMActivity
{
    class LiaDefaultFunction
    {
        static public object GetDynamicObject(int hash)
        {
            Object called = ApplicationData.Instance.dynamicTable[hash];

            Type calledType = called.GetType();

            return called;
        }

        static public void print(LiaInterpreterVar v)
        {
            string str = v.getParameter("str").getString();
            ApplicationData.Instance.box.LiaConsoleWriteLine(str);
        }

        static public void dyCall(LiaInterpreterVar v)
        {
            object called = GetDynamicObject(v.getParameter("object").getInt());

            Type calledType = called.GetType();

            ArrayList param = new ArrayList();

            int len = v.getParameter("param").getArrayLength();

            for (int i = 0; i < len; i++)
            {
                param.Add(v.getParameter("param").getArrayIndex(i));
            }

            LiaInterpreterVar s = (LiaInterpreterVar)calledType.InvokeMember(
                            v.getParameter("function").getString(),
                            BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                            null,
                            called,
                           param.ToArray());
            if (s != null)
            {
                v.setReturnVar(s);
            }
        }

        static public void dyGen(LiaInterpreterVar v)
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
                ApplicationData.Instance.box.LiaConsoleWriteLine("Dynamic type is not exist");
                return;
            }

            if (obj == null)
            {

                ApplicationData.Instance.box.LiaConsoleWriteLine("Dynamic Object Got NULL");
                return;
            }

            v.getReturnVar().setInt(obj.GetHashCode());
            ApplicationData.Instance.dynamicTable.Add(obj.GetHashCode(), obj);
        }

        static public void parseInt(LiaInterpreterVar v)
        {
            try
            {
                string str = v.getParameter("str").getString();
                LiaInterpreterVar ret = new LiaInterpreterVar(Int32.Parse(str));
                v.setReturnVar(ret);
            }
            catch (Exception e)
            {
                v.setReturnVar(new LiaInterpreterVar(-1));
            }
        }

        static public void parseFloat(LiaInterpreterVar v)
        {
            try
            {
                string str = v.getParameter("str").getString();
                LiaInterpreterVar ret = new LiaInterpreterVar(Double.Parse(str));
                v.setReturnVar(ret);
            }
            catch (Exception e)
            {
                v.setReturnVar(new LiaInterpreterVar(-1));
            }
        }
    }
}
