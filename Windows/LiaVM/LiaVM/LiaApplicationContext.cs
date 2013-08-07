using System;
using System.Windows.Forms;

namespace LiaVM
{
    class LiaApplicationContext : ApplicationContext
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.ComponentModel.IContainer components = null;
        private ContextMenu contextMenu = new ContextMenu();

        public LiaApplicationContext()
        {
            initingApplication();
            structNotifyIcon();
            createIconMenuStructure();
        }

        public void poping(String context)
        {
            notifyIcon.BalloonTipText = context;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipTitle = "실행";
            notifyIcon.ShowBalloonTip(100);
        }

        private void initingApplication()
        {
            this.components = new System.ComponentModel.Container();
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
        }

        private void structNotifyIcon()
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.Text = "Lia";
            this.notifyIcon.Visible = true;
            
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mouseClick);
            notifyIcon.Icon = new System.Drawing.Icon(LiaVM.Properties.Resources.Lia, 40, 40);
        }

        private void createIconMenuStructure()
        {
            this.components = new System.ComponentModel.Container();

            MenuItem exItem = new MenuItem();
            MenuItem infoItem = new MenuItem();
            MenuItem console = new MenuItem();

            infoItem.Index = 0;
            infoItem.Text = "개발자 정보(&I)";
            infoItem.Click += new System.EventHandler(this.infoClick);

            console.Index = 1;
            console.Text = "콘솔(&C)";
            console.Click += new System.EventHandler(this.consoelClick);

            exItem.Index = 2;
            exItem.Text = "끝내기(&X)";
            exItem.Click += new System.EventHandler(this.exitClick);

            this.contextMenu.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] {infoItem,console, exItem});

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon.ContextMenu = this.contextMenu;
        }

        private void exitClick(object sender, EventArgs e)
        {
            this.ExitThread();
        }

        private void infoClick(object sender, EventArgs e)
        {
            infoForm form = new infoForm();
            form.ShowDialog();
        }


        private void consoelClick(object sender, EventArgs e)
        {
            ConsoleForm con = new ConsoleForm();

            ApplicationData.Instance.registerConsoleForm(con);

            con.ShowDialog();
        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void OnApplicationExit(object sender, EventArgs e) 
        {
            components.Dispose();
        }
    }
}
