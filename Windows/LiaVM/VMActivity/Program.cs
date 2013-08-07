using System.Windows.Forms;
using System;

namespace VMActivity
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    MessageBox.Show("이 프로그램은 직접 실행 시킬수 없습니다.");
                    Environment.Exit(2);
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LiaMainApplicatinoContext(args[0]));
            }
            catch (Exception e)
            {
                MessageBox.Show("알 수 없는 에러가 발생하였습니다.");
            }
        }
    }
}
