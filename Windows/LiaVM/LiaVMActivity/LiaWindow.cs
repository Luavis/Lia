using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lia;
using System.Windows.Forms;

namespace LiaVMLibrary
{
    class LiaWindow
    {
        public LiaGeneralForm f;

        public LiaWindow()
        {
            f = new LiaGeneralForm();
        }

        public void ShowWindow()
        {
            //f.Invoke((MethodInvoker)delegate
            //{
                f.Text = LiaVMActivity.ApplicationData.Instance.ProgramName();
                LiaVMActivity.ApplicationData.Instance.RegistForm(f);
                f.Show();
            //});
        }

        public void SetTitle(LiaInterpreterVar v)
        {
            //f.Invoke((MethodInvoker)delegate
            //{
            f.Text = v.getString();
            //});
        }

        public void RemoveWindow()
        {
            // f.Invoke((MethodInvoker)delegate
            //{
            f.Close();
            LiaVMActivity.ApplicationData.Instance.RemoveForm(f);
            //});
        }

        public void AddComponent(LiaInterpreterVar v)
        {
            Control c = (LiaVMActivity.LiaDefaultFunction.GetDynamicObject(v.getInt()) as LiaComponent).con;

            f.Invoke((MethodInvoker)delegate
            {
                f.Controls.Add(c);
            });
        }

        public void SetMinSize(LiaInterpreterVar v, LiaInterpreterVar v2)
        {
            f.MinimumSize = new System.Drawing.Size(v.getInt(), v2.getInt());

        }

        public void SetMaxSize(LiaInterpreterVar v, LiaInterpreterVar v2)
        {
            f.MaximumSize = new System.Drawing.Size(v.getInt(), v2.getInt());
        }

        public void SetSize(LiaInterpreterVar v, LiaInterpreterVar v2)
        {
            f.Size = new System.Drawing.Size(v.getInt(), v2.getInt());
        }

        public LiaInterpreterVar Size()
        {
            LiaInterpreterVar v = new LiaInterpreterVar();
            v.setArrayIndex(0, new LiaInterpreterVar(f.Size.Width));
            v.setArrayIndex(1, new LiaInterpreterVar(f.Size.Height));

            return v;
        }
    }
}
