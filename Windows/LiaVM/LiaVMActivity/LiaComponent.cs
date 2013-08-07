using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lia;
using System.Windows.Forms;


namespace LiaVMLibrary
{
    abstract class LiaComponent
    {
        public Control con;
        public int x = 0, y = 0, w = 0, h = 0;
    }
}
