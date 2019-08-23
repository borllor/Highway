using JinRi.Notify.Monitor.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinRi.Notify.Monitor
{
    public partial class FrmTopMost : Form
    {
        private Point ptMouseCurrrnetPos, ptMouseNewPos,ptFormPos, ptFormNewPos;
        private bool blnMouseDown = false;
        internal AnchorStyles StopAanhor = AnchorStyles.None;
        public Form FrmMain { get; set; }
        public FrmTopMost()
        {
            InitializeComponent();
        }

        #region 悬浮窗体

        private void FrmTopMost_Load(object sender, EventArgs e)
        {
            this.Width = 138;
            this.Height = 36;
            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width;
            //设置窗体顶边距离主显示器位置
            this.Top = Screen.PrimaryScreen.Bounds.Height / 2;
            this.BackColor = Color.White;
            this.ShowInTaskbar = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Opacity += 0.1;
            if (this.Opacity >= 1)
                this.timer1.Enabled = false;
        }

        private void FrmTopMost_LocationChanged(object sender, EventArgs e)
        {
            mStopAnhor();
        }

        private void mStopAnhor()
        {
            if (this.Top <= 0)
            {
                StopAanhor = AnchorStyles.Top;
            }
            else if (this.Left <= 0)
            {
                StopAanhor = AnchorStyles.Left;
            }
            else if (this.Left >= Screen.PrimaryScreen.Bounds.Width - this.Width)
            {
                StopAanhor = AnchorStyles.Right;
            }
            else
            {
                StopAanhor = AnchorStyles.None;
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //如果鼠标在窗体上，则根据停靠位置显示整个窗体
            if (this.Bounds.Contains(Cursor.Position))
            {
                switch (this.StopAanhor)
                {
                    case AnchorStyles.Top:
                        this.Location = new Point(this.Location.X, 0);
                        break;
                    case AnchorStyles.Bottom:
                        this.Location = new Point(this.Location.X, Screen.PrimaryScreen.Bounds.Height - this.Height);
                        break;
                    case AnchorStyles.Left:
                        this.Location = new Point(0, this.Location.Y);
                        break;
                    case AnchorStyles.Right:
                        this.Location = new Point(Screen.PrimaryScreen.Bounds.Width -
                        this.Width, this.Location.Y);
                        break;
                }
            }
            else //如果鼠标离开窗体，则根据停靠位置隐藏窗体，但须留出部分窗体边缘以便鼠标选中窗体
            {
                switch (this.StopAanhor)
                {
                    case AnchorStyles.Top:
                        this.Location = new Point(this.Location.X, (this.Height - 3) * (-1));
                        break;
                    case AnchorStyles.Bottom:
                        this.Location = new Point(this.Location.X, Screen.PrimaryScreen.Bounds.Height - 5);
                        break;
                    case AnchorStyles.Left:
                        this.Location = new Point((-1) * (this.Width - 4), this.Location.Y);
                        break;
                    case AnchorStyles.Right:
                        this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 4, this.Location.Y);
                        break;
                }
            }
        } 

        #endregion

        #region 无边框窗体拖拽

        private void FrmTopMost_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnMouseDown)
            {
                ptMouseNewPos = Control.MousePosition;
                ptFormNewPos.X = ptMouseNewPos.X - ptMouseCurrrnetPos.X + ptFormPos.X;
                ptFormNewPos.Y = ptMouseNewPos.Y - ptMouseCurrrnetPos.Y + ptFormPos.Y;
                Location = ptFormNewPos;
                ptFormPos = ptFormNewPos;
                ptMouseCurrrnetPos = ptMouseNewPos;

            }
        }

        private void FrmTopMost_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                blnMouseDown = false;
        }

        private void FrmTopMost_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                blnMouseDown = true;
                ptMouseCurrrnetPos = Control.MousePosition;
                ptFormPos = Location;

            }
        } 

        #endregion

        #region 圆角处理

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RenderHelper.SetFormRoundRectRgn(this, 4);
        }

        #endregion

        private void FrmTopMost_DoubleClick(object sender, EventArgs e)
        {
            ShowMainForm();
        }


        private void ShowMainForm()
        {
            if (FrmMain == null)
            {
                FrmMain = new FrmMain(this);
            }
            FrmMain.Show();
            FrmMain.Focus();
        }

       
        #region 桌面右下角图标

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowMainForm();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {

            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        } 

        #endregion

        private void cMenuForTopMost_Click(object sender, EventArgs e)
        {
            if (FrmMain != null)
            {
                FrmMain.Close();
            }
            this.Close();
        }

    }
}
