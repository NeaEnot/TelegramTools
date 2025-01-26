using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TL;

namespace TelegrammParser
{
    public partial class MainWindow : Window
    {
        private Regex phoneRegex;
        private TelegramLayer telegramLayer;

        public MainWindow()
        {
            InitializeComponent();

            phoneRegex = new Regex("^\\+?[7-8][0-9]{10}$");
            telegramLayer = new TelegramLayer();

            dpDateTo.SelectedDate = DateTime.Now;
        }

        private void tbPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (phoneRegex.IsMatch(tbPhone.Text))
                btnConnect.IsEnabled = true;
            else
                btnConnect.IsEnabled = false;
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await telegramLayer.Connect(tbPhone.Text);

                IEnumerable<UserModel> chats = await telegramLayer.GetChats();
                cbChats.Items.Clear();
                cbChats.ItemsSource = chats;

                cbChats.IsEnabled = true;
                dpDateFrom.IsEnabled = true;
                dpDateTo.IsEnabled = true;
                btnDownload.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserModel selectedUser = cbChats.SelectedItem as UserModel;

                List<MessageModel> messages =
                    await telegramLayer.DownloadChat(cbChats.SelectedItem as UserModel, dpDateFrom.SelectedDate.Value, dpDateTo.SelectedDate.Value);
                messages.Reverse();

                string history = "";
                foreach (MessageModel msg in messages)
                    history += msg.ToString();

                using (StreamWriter writer = new StreamWriter(selectedUser.Name + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt"))
                {
                    writer.Write(history);
                }

                MessageBox.Show("Готово", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
