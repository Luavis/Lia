using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Lia;

namespace LiaVMLibrary
{
    class LiaLabel : LiaComponent
    {
        public Label l;

        public LiaLabel()
        {
            l = new Label();
            //l.Invoke((MethodInvoker)delegate
            //{
                l.AutoSize = true;
                l.Location = new System.Drawing.Point(0, 0);
                l.Name = this.GetHashCode().ToString();
                l.Size = new System.Drawing.Size(40, 10);
                l.TabIndex = 0;
                l.Text = "";

                this.con = (Control)l;
            //});
        }

        public void SetText(LiaInterpreterVar v)
        {
            //l.Invoke((MethodInvoker)delegate
            //{
                l.Text = v.getString();
            //});
        }

        public void SetSize(LiaInterpreterVar v, LiaInterpreterVar v2)
        {
            //l.Invoke((MethodInvoker)delegate
            //{
                l.Size = new System.Drawing.Size(v.getInt(), v2.getInt());
            //});
        }

        public void SetPosition(LiaInterpreterVar v, LiaInterpreterVar v2)
        {
            //l.Invoke((MethodInvoker)delegate
            //{
                l.Location = new System.Drawing.Point(v.getInt(), v2.getInt());
            //});
        }

        public LiaInterpreterVar Size()
        {
            LiaInterpreterVar v = new LiaInterpreterVar();
            v.setArrayIndex(0, new LiaInterpreterVar(l.Size.Width));
            v.setArrayIndex(1, new LiaInterpreterVar(l.Size.Height));

            return v;
        }

        public LiaInterpreterVar Position()
        {
            LiaInterpreterVar v = new LiaInterpreterVar();
            v.setArrayIndex(0, new LiaInterpreterVar(l.Location.X));
            v.setArrayIndex(1, new LiaInterpreterVar(l.Location.Y));

            return v;
        }

        public LiaInterpreterVar Text()
        {
            LiaInterpreterVar v = new LiaInterpreterVar(l.Text);
            return v;
        }
    }
}
