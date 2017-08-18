using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace modelly
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        public MessageDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ScvStackTrace.Visibility == System.Windows.Visibility.Collapsed)
            {
                this.Height = this.Height + 150;
                ScvStackTrace.Visibility = System.Windows.Visibility.Visible;
                BtnLinkTrace.Content = "View stack trace -";
            }
            else
            {
                this.Height = this.Height - 150;
                ScvStackTrace.Visibility = System.Windows.Visibility.Collapsed;
                BtnLinkTrace.Content = "View stack trace +";
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
