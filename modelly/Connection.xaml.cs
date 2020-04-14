using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace modelly
{
    using modelly;
    using MySql.Data.MySqlClient;
    using System.Data;

    using System.Deployment.Application;
    using System.Reflection;
    /// <summary>
    /// Interaction logic for Connection.xaml
    /// </summary>
    /// 
    /*
     * 
     */
    public partial class Connection : Window
    {
        public string uLogin { get; set; }
        public string uPassword { get; set; }
        public string serverName { get; set; }
        public int serverPort { get; set; }
        public bool IsDotNetCore { get; set; }
        public string connectionString { get; set; }
        public int serverType { get; set; }
        public List<string> ServerTypes { get; set; }
        public bool IsChild { get; set; }
        public Connection()
        {
            InitializeComponent();

            IsChild = false;
        }
        public Connection(bool _IsChild)
        {
            InitializeComponent();

            IsChild = _IsChild;
        }
        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ckxTustedConnection.IsChecked = true;
            tbxCnxString.Focus();
            ServerTypes = new List<string> { "SQL Server", "MySQL", "PostGreSql" };
            cbxServerType.ItemsSource = ServerTypes;
            cbxServerType.SelectedIndex = 0;
            serverPort = 3306;
            tbxServerPort.Text = serverPort.ToString();
            tbxCnxString.Text = "pesnic";

            vers.Content = getRunningVersion();
        }
        private void Set_Visibility(int serverType)
        {
            switch (serverType)
            {
                case 0: //"sql server":
                    {
                        lblServerName.Content = "Server name";
                        ckxTustedConnection.IsChecked = true;
                        tbxUserLogin.IsEnabled = false;
                        tbxUserPassword.IsEnabled = false;
                        ckxTustedConnection.Visibility = System.Windows.Visibility.Visible;
                        tbxServerPort.Visibility = System.Windows.Visibility.Hidden;
                        lblServerPort.Visibility = System.Windows.Visibility.Hidden;
                        tbxCnxString.Text = Environment.MachineName;
                        break;
                    }
                case 1: //"mysql":
                    {
                        lblServerName.Content = "Server host";
                        ckxTustedConnection.IsChecked = false;
                        tbxUserLogin.IsEnabled = true;
                        tbxUserPassword.IsEnabled = true;
                        ckxTustedConnection.Visibility = System.Windows.Visibility.Hidden;
                        serverPort = 3306;
                        tbxServerPort.Visibility = System.Windows.Visibility.Visible;
                        lblServerPort.Visibility = System.Windows.Visibility.Visible;
                        tbxCnxString.Text = "localhost";
                        break;
                    }
                case 2: //"postgre":
                    {
                        lblServerName.Content = "Server host";
                        ckxTustedConnection.IsChecked = false;
                        tbxUserLogin.IsEnabled = true;
                        tbxUserPassword.IsEnabled = true;
                        ckxTustedConnection.Visibility = System.Windows.Visibility.Hidden;
                        serverPort = 5432;
                        tbxServerPort.Visibility = System.Windows.Visibility.Visible;
                        lblServerPort.Visibility = System.Windows.Visibility.Visible;
                        tbxCnxString.Text = "";
                        break;
                    }

            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            uLogin = tbxUserLogin.Text;
            uPassword = tbxUserPassword.Password;
            serverName = tbxCnxString.Text;
            IsDotNetCore = false;
            int sType = cbxServerType.SelectedIndex;
            if (sType > -1)
            {
                serverType = sType;
                switch (sType)
                {
                    case 0://SQL Server
                        {
                            if (ckxTustedConnection.IsChecked == true)
                            {
                                connectionString = "Server=" + serverName + ";Trusted_Connection=True;Connection Timeout=30;";
                            }
                            else
                            {
                                connectionString = "Server=" + serverName + ";User Id=" + uLogin + ";Password=" + uPassword + ";Connection Timeout=30;";
                            }
                            break;
                        }
                    case 1: // MySQL
                        {
                            connectionString = "Server=" + serverName + ";Port=" + serverPort + ";UId=" + uLogin + ";Pwd=" + uPassword + ";Connection Timeout=30;";
                            break;
                        }
                }
                //Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;
                //Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;
                string errMessage;
                try
                {
                    if (ConnectToServer(connectionString, sType, out errMessage))
                    {
                        //MessageBox.Show("Just get connected :)");
                        if (!IsChild)
                        {
                            MainWindow mw = new MainWindow(connectionString, serverType, IsDotNetCore);
                            mw.Show();
                        }
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cannot connect to server!\nPlease, make sure that connection credentials are correct and try again");
                    }
                }
                catch (Exception ex)
                {
                    var msg = new MessageDialog();
                    msg.Owner = this;
                    msg.LblMessage.Text = "Cannot connect to server!\nPlease, make sure that connection credentials are correct and try again";
                    msg.TbkStackTrace.Text = ex.Message + Environment.NewLine + ex.StackTrace;
                    msg.ShowDialog();
                }
            }
        }

        public bool ConnectToServer(string cnxString, int srvType, out string errMessage)
        {
            errMessage = "";
            try
            {
                bool result = false;
                switch (srvType)
                {
                    case 0: // sql server
                        {
                            SqlConnection connection = new SqlConnection(cnxString);
                            using (new WaitCursor())
                            {
                                connection.Open();
                                result = true;
                            }
                            break;
                        }
                    case 1: // mysql
                        {
                            MySqlConnection connection = new MySqlConnection(cnxString);
                            using (new WaitCursor())
                            {
                                connection.Open();
                                result = true;
                            }
                            break;
                        }
                    case 2://postgre
                        {
                            MySqlConnection connection = new MySqlConnection(cnxString);
                            using (new WaitCursor())
                            {
                                connection.Open();
                                result = true;
                            }
                            break;
                        }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
                //errMessage = ex.Message;
                //return false;
            }
        }

        private void ckxTustedConnection_Checked(object sender, RoutedEventArgs e)
        {
            tbxUserLogin.IsEnabled = !(bool)ckxTustedConnection.IsChecked;
            tbxUserPassword.IsEnabled = !(bool)ckxTustedConnection.IsChecked;
        }

        private void ckxTustedConnection_Click(object sender, RoutedEventArgs e)
        {
            tbxUserLogin.IsEnabled = !(bool)ckxTustedConnection.IsChecked;
            tbxUserPassword.IsEnabled = !(bool)ckxTustedConnection.IsChecked;
        }

        public class WaitCursor : IDisposable
        {
            private Cursor _previousCursor;

            public WaitCursor()
            {
                _previousCursor = Mouse.OverrideCursor;

                Mouse.OverrideCursor = Cursors.Wait;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Mouse.OverrideCursor = _previousCursor;
            }

            #endregion
        }

        private void cbxServerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serverType = cbxServerType.SelectedIndex;
            Set_Visibility(serverType);
        }

        private void tbx_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Connect_Click(sender, e);
            }
        }
    }
}
