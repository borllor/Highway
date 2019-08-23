using JinRi.Notify.Monitor.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JinRi.Notify.Monitor
{
    public partial class FrmMain : Form
    {
        private FrmTopMost frmTopMost;
        private static string MONITORCENT_URI = ConfigurationManager.AppSettings["MONITORCENT_URI"];
        private static bool isFullScreen = false;
        private static int rowCount = 2;
        private static int columnCount = 2;
        private static Dictionary<string, WebBrowser> browserDic = new Dictionary<string, WebBrowser>();
        private static string DEFAULT_METRICS_KEY = ConfigurationManager.AppSettings["DEFAULT_METRICS_KEY"];
        private static string DEFAULT_ZOOM = ConfigurationManager.AppSettings["DEFAULT_ZOOM"];
        private const string HISTORY_FILE_NAME = "history.txt";
        public FrmMain()
        {
            InitializeComponent();
            tableLayout.ColumnCount = 2;
        }

        public FrmMain(FrmTopMost frmTopMost)
        {
            InitializeComponent();
            this.frmTopMost = frmTopMost;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            string metricsKey = ReadMetricsKey();
            if (!string.IsNullOrEmpty(metricsKey))
            {
                txtMetrics.Text = metricsKey;
                btnSearch_Click(btnSearch, new EventArgs());
            }
            else
            {
                InitWebBrowser(DEFAULT_METRICS_KEY);
            }
            InitMetricsKey();
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            
            tableLayout.Controls.Clear();
            browserDic.Clear();
            string searchKey = txtMetrics.Text;
            if (string.IsNullOrEmpty(searchKey))
            {
                MessageBox.Show("请输入需要查询的度量名称(多个以逗号分隔)");
                return;
            }
            string[] keys = searchKey.Split(',');
            tableLayout.RowCount = keys.Length % tableLayout.ColumnCount == 0 ?
                    keys.Length / tableLayout.ColumnCount : keys.Length / tableLayout.ColumnCount + 1;
            tableLayout.RowCount = tableLayout.RowCount <= 2 ? 2 : tableLayout.RowCount;
            tableLayout.RowStyles.Add(new RowStyle() { SizeType = System.Windows.Forms.SizeType.Percent });
            for (int i = 0; i < keys.Length; i++)
            {
                InitWebBrowser(keys[i]);
            }
        }

        /// <summary>
        /// 初始化WebBrowser
        /// </summary>
        /// <param name="metricsKey"></param>
        private void InitWebBrowser(string metricsKey)
        {
            WebBrowser browser = new WebBrowser();
            tableLayout.Controls.Add(browser);
            browser.Navigate(MONITORCENT_URI + "?metricsKey=" + metricsKey + "&t=" + DateTime.Now.Ticks);
            browser.Dock = DockStyle.Fill;
            browser.DocumentCompleted += (_sender, _e) =>
            {
                var _browser = _sender as WebBrowser;
                if (_browser.ReadyState != WebBrowserReadyState.Complete) return;
                _browser.Document.Body.Style = DEFAULT_ZOOM;
                var container = _browser.Document.GetElementById("container");
                if (container != null)
                {
                    container.DoubleClick += container_DoubleClick;
                }
            };
            browserDic[metricsKey] = browser;
        }

        /// <summary>
        /// 初始化MetricsKey
        /// </summary>
        private void InitMetricsKey()
        {
            var metricsKeySetting = ConfigurationManager.AppSettings["MetricsKey"];
            if (!string.IsNullOrEmpty(metricsKeySetting))
            {
                txtMetrics.AutoCompleteCustomSource.AddRange(metricsKeySetting.Split('|'));
            }
            else
            {
                txtMetrics.AutoCompleteCustomSource.AddRange(new string[] { "Flight.Booking.SOA.GetFlight.ErrorCount", "Flight.Booking.SOA.GetFlight" });
            }
        }

        void container_DoubleClick(object sender, HtmlElementEventArgs e)
        {
            tableLayout.Controls.Clear();
            var document = ((HtmlElement)sender).Parent.Document;
            var metricsKey = document.GetElementById("metricsKey").GetAttribute("value");
            if (!isFullScreen)
            {
                rowCount = tableLayout.RowCount;
                columnCount = tableLayout.ColumnCount;
                tableLayout.Controls.Add(browserDic[metricsKey]);
            }
            else
            {
                tableLayout.Controls.AddRange(browserDic.Values.ToArray<WebBrowser>());
            }
            tableLayout.RowCount = isFullScreen ? rowCount : 1;
            tableLayout.ColumnCount = isFullScreen ? columnCount : 1;
            document.Body.Style = isFullScreen ? DEFAULT_ZOOM : "zoom:100%";
            isFullScreen = !isFullScreen;
           
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmTopMost.FrmMain = null;
            SaveMetricsKey(txtMetrics.Text);
        }

        /// <summary>
        /// 保存搜索历史
        /// </summary>
        /// <param name="metricsKey"></param>
        private void SaveMetricsKey(string metricsKey)
        {
            try
            {
                string fileName = Environment.CurrentDirectory + "\\" + HISTORY_FILE_NAME;
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(metricsKey);
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 加载历史搜索
        /// </summary>
        /// <returns></returns>
        private string ReadMetricsKey()
        {
            string metricsKey = string.Empty;
            try
            {
                string fileName = Environment.CurrentDirectory + "\\" + HISTORY_FILE_NAME;
                metricsKey = File.ReadAllText(fileName);
            }
            catch (Exception)
            {
            }
            return metricsKey;
        }
    }
}
