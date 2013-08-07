using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiaVMLibrary
{
    public partial class LiaGeneralForm : Form
    {
        public LiaGeneralForm()
        {
            InitializeComponent();
        }

        private void LiaGeneralForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            LiaVMActivity.ApplicationData.Instance.RemoveForm(this);
        }
    }
}
