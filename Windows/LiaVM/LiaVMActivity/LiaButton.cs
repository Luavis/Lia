using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Lia;


namespace LiaVMLibrary
{
    class LiaButton : LiaComponent
    {
        Button b;
        String selector = null;

        public LiaButton()
        {
            b = new Button();

            //b.Invoke((MethodInvoker)delegate
           // {
                this.b.Location = new System.Drawing.Point(60, 74);
                this.b.Name = "button1";
                this.b.Size = new System.Drawing.Size(75, 23);
                this.b.TabIndex = 0;
                this.b.Text = "button1";
                this.b.UseVisualStyleBackColor = true;
                this.con = (Control)b;
            //});
        }

        private void SetEventClickOccur(object sender, EventArgs e)
        {
            if (selector != null)
            {
                LiaVMActivity.ApplicationData.Instance.box.li.call(selector);
            }
        }

        public void setClick(LiaInterpreterVar v)
        {
            selector = v.getString();
            b.Click += new EventHandler(this.SetEventClickOccur);
        }

        public void SetText(LiaInterpreterVar v)
        {
            //b.Invoke((MethodInvoker)delegate
            //{
                b.Text = v.getString();
           // });
            
        }

        public void SetSize(LiaInterpreterVar v, LiaInterpreterVar v2)
        {
            //b.Invoke((MethodInvoker)delegate
            //{
                b.Size = new System.Drawing.Size(v.getInt(), v2.getInt());
            //});
        }

        public void SetPosition(LiaInterpreterVar v, LiaInterpreterVar v2)
        {
            //b.Invoke((MethodInvoker)delegate
            //{
                b.Location = new System.Drawing.Point(v.getInt(), v2.getInt());
            //});
        }

        public LiaInterpreterVar Size()
        {
            LiaInterpreterVar v = new LiaInterpreterVar();
            v.setArrayIndex(0, new LiaInterpreterVar(b.Size.Width));
            v.setArrayIndex(1, new LiaInterpreterVar(b.Size.Height));

            return v;
        }

        public LiaInterpreterVar Position()
        {
            LiaInterpreterVar v = new LiaInterpreterVar();
            v.setArrayIndex(0, new LiaInterpreterVar(b.Location.X));
            v.setArrayIndex(1, new LiaInterpreterVar(b.Location.Y));

            return v;
        }

        public LiaInterpreterVar Text()
        {
            LiaInterpreterVar v = new LiaInterpreterVar(b.Text);
            return v;
        }
    }
}
