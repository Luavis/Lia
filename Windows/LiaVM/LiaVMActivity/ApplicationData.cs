using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace LiaVMActivity
{
    class ApplicationData
    {
        static ApplicationData instance = null;
        static readonly object padlock = new object();
        public LiaToolBox box;
        private ArrayList registedForm = new ArrayList();
        public Dictionary<int, Object> dynamicTable = new Dictionary<int, Object>();

        public String Code()
        {
            return box.Code;
        }
        public String ProgramName()
        {
            return box.ProgramName;
        }
        public void RegistForm(Form f)
        {
            registedForm.Add(f);
        }

        public void RemoveForm(Form f)
        {
            registedForm.Remove(f);
        }

        public int CountRegistedForm()
        {
            return registedForm.Count;
        }

        public static ApplicationData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ApplicationData();
                    }
                    return instance;
                }
            }
        }
    }
}
