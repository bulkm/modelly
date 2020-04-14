using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace modelly
{
    using modelly;
    using MySql.Data.MySqlClient;
    using System.Data;

    using System.Deployment.Application;
    using System.Reflection;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ProjectNamespace { get; set; }
        public List<Database> databaseList;
        public string connectionString { get; set; }
        public int serverType { set; get; }
        public bool IsDotNetCore { get; set; }
        ObservableCollection<Table> TableList = new ObservableCollection<Table>();
        public MainWindow()
        {
            InitializeComponent();
            databaseList = new List<Database> { new Database { Name = "Performrx", ServerName = "l4" }, new Database { Name = "nddf", ServerName = "local" } };



            tbxConfigNamespace.GotFocus += RemoveText;
            tbxConfigNamespace.LostFocus += AddText;
            tbxHelpersNamespace.GotFocus += RemoveText2;
            tbxHelpersNamespace.LostFocus += AddText2;

            ckxConfigFiles.IsChecked = false;
            AddText(null, null);
            ckxHelpersFiles.IsChecked = false;
            AddText2(null, null);
        }

        public MainWindow(string cnxString, int svrType, bool isDNC)
            : this()
        {
            connectionString = cnxString;
            serverType = svrType;
            IsDotNetCore = isDNC;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetConnected(false);
            vers.Content = getRunningVersion();
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
        private void connectMenu_Click(object sender, RoutedEventArgs e)
        {
            GetConnected(true);
        }

        private void closeMenu_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public class Database
        {
            public string Name { get; set; }
            public string ServerName { get; set; }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void RemoveText(object sender, EventArgs e)
        {
            tbxConfigNamespace.Text = "";
            tbxConfigNamespace.Foreground = Brushes.Black;
            //tbxConfigNamespace.FontSize = 12;
        }
        public void AddText(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbxConfigNamespace.Text))
            {
                tbxConfigNamespace.Foreground = Brushes.Gray;
                //tbxConfigNamespace.FontSize = 10;
                tbxConfigNamespace.Text = "Leave empty for default";
            }
        }
        public void RemoveText2(object sender, EventArgs e)
        {
            tbxHelpersNamespace.Text = "";
            tbxHelpersNamespace.Foreground = Brushes.Black;
            //tbxHelpersNamespace.FontSize = 12;
        }
        public void AddText2(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(tbxHelpersNamespace.Text))
            {
                tbxHelpersNamespace.Foreground = Brushes.Gray;
                //tbxHelpersNamespace.FontSize = 10;
                tbxHelpersNamespace.Text = "Leave empty for default";
            }
        }

        #region methods
        private void SetDatabases(List<string> dtList)
        {
            using (new WaitCursor())
            {
                cbxDatabaseList.ItemsSource = dtList;
            }
        }
        public void GetConnected(bool IsChild)
        {
            try
            {
                if (IsChild)
                {
                    Connection cnx = new Connection(true);
                    cnx.ShowDialog();
                    connectionString = cnx.connectionString;
                    serverType = cnx.serverType;
                    IsDotNetCore = cnx.IsDotNetCore;
                }
                if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrWhiteSpace(connectionString))
                {
                    using (new WaitCursor())
                    {
                        switch (serverType)
                        {
                            case 0: // sqlserver
                                {
                                    List<string> dtList = GetDatabasesName_SqlServer(connectionString);
                                    SetDatabases(dtList);
                                    break;
                                }
                            case 1: // mysql
                                {
                                    List<string> dtList = GetDatabasesName_MySql(connectionString);
                                    SetDatabases(dtList);
                                    break;
                                }
                            case 2: // postgre
                                {
                                    List<string> dtList = GetDatabasesName_MySql(connectionString);
                                    SetDatabases(dtList);
                                    break;
                                }
                        }
                        cbxDatabaseList.SelectedIndex = 0;
                        connectionString += "Database=" + cbxDatabaseList.SelectedItem.ToString() + ";";
                    }
                }
                else
                {
                    MessageBox.Show("Cannot connect to server!\nPlease, check credentials and retry again");
                }
            }
            catch (Exception ex)
            {
                var msg = new MessageDialog();
                msg.Owner = this;
                msg.LblMessage.Text = "Cannot connect to database.\nPlease check provided information and retry again";
                msg.TbkStackTrace.Text = ex.Message + Environment.NewLine + ex.StackTrace;
                msg.ShowDialog();
            }

        }
        public static List<string> GetDatabasesName_SqlServer(string cnxString)
        {
            try
            {
                List<string> DBName = new List<string>();

                using (SqlConnection connection = new SqlConnection(cnxString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [name] FROM master.dbo.sysdatabases WHERE dbid >= 4 ";
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DBName.Add(reader.GetString(0));
                        }
                    }
                }
                return DBName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<string> GetDatabasesName_MySql(string cnxString)
        {
            try
            {
                List<string> DBName = new List<string>();

                using (MySqlConnection connection = new MySqlConnection(cnxString))
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "select schema_name from information_schema.SCHEMATA ";
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DBName.Add(reader.GetString(0));
                        }
                    }
                }
                return DBName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> GetDatatablesName_SqlServer(string cnxString, string DbName)
        {
            try
            {
                List<string> TableNames = new List<string>();

                using (SqlConnection connection = new SqlConnection(cnxString))
                {
                    connection.Open();
                    DataTable schema = connection.GetSchema("Tables");
                    foreach (DataRow row in schema.Rows)
                    {
                        TableNames.Add(FirstCharToUpper(row[2].ToString()));
                    }
                    return TableNames;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> GetDatatablesName_MySql(string cnxString, string DbName)
        {
            try
            {
                List<string> TableNames = new List<string>();
                using (MySqlConnection connection = new MySqlConnection(cnxString))
                {
                    connection.Open();
                    DataTable schema = connection.GetSchema("Tables");
                    foreach (DataRow row in schema.Rows)
                    {
                        TableNames.Add(FirstCharToUpper(row[2].ToString()));
                    }
                    return TableNames;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string FirstCharToUpper(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                return input.First().ToString().ToUpper() + input.Substring(1);
            }
            else
                return input;
        }

        public class Table : INotifyPropertyChanged
        {
            public string Name { get; set; }
            private bool _IsChecked;
            public bool IsChecked
            {
                get
                {
                    return _IsChecked;
                }
                set
                {
                    if (value != IsChecked)
                    {
                        _IsChecked = value;
                        NotifyPropertyChanged("IsChecked");
                    }
                }
            }

            private void NotifyPropertyChanged(string info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
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
        #endregion

        private void cbxDatabaseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // clear tables list
                TableList = null;
                TableList = new ObservableCollection<Table>();

                // populate listbox
                int dbIdx = connectionString.IndexOf("Database=");
                if (dbIdx >= 0)
                {
                    connectionString = connectionString.Substring(0, dbIdx);
                    if (((ComboBox)sender).SelectedItem != null)
                        connectionString += "Database=" + ((ComboBox)sender).SelectedItem.ToString() + ";";
                }
                else
                {
                    if (((ComboBox)sender).SelectedItem != null)
                        connectionString += "Database=" + ((ComboBox)sender).SelectedItem.ToString() + ";";
                }

                List<string> tblList = null;
                switch (serverType)
                {
                    case 0: // sqlserver
                        {
                            if (((ComboBox)sender).SelectedItem != null)
                                tblList = GetDatatablesName_SqlServer(connectionString, ((ComboBox)sender).SelectedItem.ToString());
                            break;
                        }
                    case 1: // mysql
                        {
                            if (((ComboBox)sender).SelectedItem != null)
                                tblList = GetDatatablesName_MySql(connectionString, ((ComboBox)sender).SelectedItem.ToString());
                            break;
                        }
                    case 2: // postgre
                        {
                            if (((ComboBox)sender).SelectedItem != null)
                                tblList = GetDatatablesName_SqlServer(connectionString, ((ComboBox)sender).SelectedItem.ToString());
                            break;
                        }
                }
                if (tblList != null && tblList.Count > 0)
                {
                    foreach (var t in tblList)
                    {
                        TableList.Add(new Table { Name = t, IsChecked = false });
                    }
                    if (TableList != null && TableList.Count > 0)
                    {
                        TableList = new ObservableCollection<Table>(TableList.OrderBy(T => T.Name));
                    }
                    ltxDatatableList.ItemsSource = TableList;
                }
            }
            catch (Exception ex)
            {
                var msg = new MessageDialog();
                msg.Owner = this;
                msg.LblMessage.Text = "Cannot retrieve database objects.\nPlease check credentials and retry again";
                msg.TbkStackTrace.Text = ex.Message + Environment.NewLine + ex.StackTrace;
                msg.ShowDialog();
            }
        }
        private void btnGenerateFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var SelectedTblCount = (from T in TableList where T.IsChecked == true select T.Name).ToList();
                string destFolder = "";
                if (SelectedTblCount != null && SelectedTblCount.Count > 0)
                {
                    using (new WaitCursor())
                    {
                        switch (serverType)
                        {
                            case 0: // Sqlserver
                                {
                                    destFolder = ErnstUtils.Helpers.EntityGenerator.GenerateFiles(connectionString, SelectedTblCount, tbxProjectNamespace.Text, ErnstUtils.Helpers.EntityGenerator.DatabaseType.SqlServer, IsDotNetCore, (bool)ckxConfigFiles.IsChecked, (bool)ckxHelpersFiles.IsChecked, tbxConfigNamespace.Text, tbxHelpersNamespace.Text);
                                    break;
                                }
                            case 1: // mysql
                                {
                                    destFolder = ErnstUtils.Helpers.EntityGenerator.GenerateFiles(connectionString, SelectedTblCount, tbxProjectNamespace.Text, ErnstUtils.Helpers.EntityGenerator.DatabaseType.MySql, false, (bool)ckxConfigFiles.IsChecked, (bool)ckxHelpersFiles.IsChecked, tbxConfigNamespace.Text, tbxHelpersNamespace.Text);
                                    break;
                                }
                        }

                    }
                    if (!string.IsNullOrEmpty(destFolder) && !string.IsNullOrWhiteSpace(destFolder))
                    {
                        MessageBoxResult r = MessageBox.Show("Files generated successfully!\nWould you like to open destination folder?", "done", MessageBoxButton.YesNo);
                        if (r == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start(destFolder);
                        }
                    }
                    else
                    {
                        MessageBox.Show("An error occurs while generating files.\nPlease retry a few minutes later.");
                    }
                }
                else
                {
                    MessageBox.Show("No tables selected!");
                }
            }
            catch (Exception ex)
            {
                var msg = new MessageDialog();
                msg.Owner = this;
                msg.LblMessage.Text = "Cannot generate files.\nPlease check provided information and retry again";
                msg.TbkStackTrace.Text = ex.Message + Environment.NewLine + ex.StackTrace;
                msg.ShowDialog();
            }
        }
        private void ckxSelectAllTables_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TableList != null && TableList.Count > 0)
                {
                    for (int i = 0; i < TableList.Count; i++)
                    {
                        TableList[i].IsChecked = (bool)((CheckBox)sender).IsChecked;
                    }
                }
            }
            catch (Exception ex)
            {

                var msg = new MessageDialog();
                msg.Owner = this;
                msg.LblMessage.Text = "Cannot select all tables.\nPlease retry again";
                msg.TbkStackTrace.Text = ex.Message + Environment.NewLine + ex.StackTrace;
                msg.ShowDialog();
            }
        }

        private void ckxHelpersFiles_Click(object sender, RoutedEventArgs e)
        {

            //if (ckxHelpersFiles.IsChecked == false)
            //{
            //    LblHelpersNameSpace.Visibility = System.Windows.Visibility.Visible;
            //    tbxHelpersNamespace.Visibility = System.Windows.Visibility.Visible;
            //    AddText2((object)tbxHelpersNamespace, (EventArgs)e);
            //}
            //else
            //{
            //    LblHelpersNameSpace.Visibility = System.Windows.Visibility.Collapsed;
            //    tbxHelpersNamespace.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        private void ckxConfigFiles_Click(object sender, RoutedEventArgs e)
        {
            //if (ckxConfigFiles.IsChecked == false)
            //{
            //    LblConfigNameSpace.Visibility = System.Windows.Visibility.Visible;
            //    tbxConfigNamespace.Visibility = System.Windows.Visibility.Visible;
            //    AddText((object)tbxConfigNamespace, (EventArgs)e);
            //}
            //else
            //{
            //    LblConfigNameSpace.Visibility = System.Windows.Visibility.Collapsed;
            //    tbxConfigNamespace.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }
    }
}
