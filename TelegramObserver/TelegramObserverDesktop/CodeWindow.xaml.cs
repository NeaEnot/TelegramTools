using System.Windows;

namespace TelegramObserverDesktop
{
    public partial class CodeWindow : Window
    {
        public string Code { get; private set; }

        public CodeWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Code = tbCode.Text;
            DialogResult = true;
        }
    }
}
