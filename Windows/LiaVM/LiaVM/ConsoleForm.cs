using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace LiaVM
{
    public partial class ConsoleForm : Form
    {
        public ConsoleForm()
        {
            InitializeComponent();
            reloadConsole();
        }

        private void _reloadConsole()
        {
            ArrayList a = ApplicationData.Instance.consoleContext;
            Array temp = a.ToArray();
            consoleBox.Text = "";
            
            if (temp == null)
            {
                return;
            }

            for (int i = 0; i < temp.Length; i++)
            {
                consoleBox.Text += (String)(temp.GetValue(i)) + "\r\n";
            }
        }

        public void reloadConsole()
        {
            this._reloadConsole();
        }

        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationData.Instance.removeConsoleForm(this);
        }


    }
}
