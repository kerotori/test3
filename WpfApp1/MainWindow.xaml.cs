using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var view1 = new 画面1.MainWindow();
            view1.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var view2 = new 画面2.MainWindow();
            view2.Show();
        }
    }
}
