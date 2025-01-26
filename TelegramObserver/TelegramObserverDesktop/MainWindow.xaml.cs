using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLibrary;
using TL;

namespace TelegramObserverDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TelegramClient client;
        private static Config config;

        private IEnumerable<ChatModel> allChats;
        private List<ChatModel> selectedChats;

        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnConnecnt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                config = Config.Load();

                client = new TelegramClient(config.ApiId, config.ApiHash, config.PhoneNumber);
                await client.Connect(AskCode);

                await GetChats();
                Load();

                lbAllChats.IsEnabled = true;
                lbSelectedChats.IsEnabled = true;
                btnSave.IsEnabled = true;
                btnStartObserve.IsEnabled = true;
                cbObserveUnknown.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static Func<string> AskCode = () =>
        {
            string code = Application.Current.Dispatcher.Invoke(() =>
            {
                CodeWindow window = new CodeWindow();

                if (window.ShowDialog() == true)
                    return window.Code;

                return "";
            });

            return code;
        };

        private async void Load()
        {
            if (!File.Exists("selected.json"))
            {
                selectedChats = new List<ChatModel>();
                return;
            }

            using (StreamReader reader = new StreamReader("selected.json"))
            {
                string json = reader.ReadToEnd();
                List<ChatModel> restored = JsonConvert.DeserializeObject<List<ChatModel>>(json, jsonSerializerSettings);

                selectedChats = restored ?? new List<ChatModel>();
            }

            lbSelectedChats.Items.Clear();
            lbSelectedChats.ItemsSource = selectedChats;
        }

        private async Task GetChats()
        {
            allChats = await client.GetChats();
            lbAllChats.Items.Clear();
            lbAllChats.ItemsSource = allChats;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("selected.json"))
            {
                string json = JsonConvert.SerializeObject(selectedChats, jsonSerializerSettings);
                writer.WriteAsync(json);
            }
        }

        private async void lbAllChats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChatModel chatModel = lbAllChats.SelectedItem as ChatModel;
            if (!selectedChats.Contains(chatModel))
            {
                selectedChats.Add(chatModel);

                lbSelectedChats.ItemsSource = null;
                lbSelectedChats.ItemsSource = selectedChats;
            }
        }

        private async void lbSelectedChats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChatModel chatModel = lbSelectedChats.SelectedItem as ChatModel;

            selectedChats.Remove(chatModel);

            lbSelectedChats.ItemsSource = null;
            lbSelectedChats.ItemsSource = selectedChats;
        }

        private async void btnStartObserve_Click(object sender, RoutedEventArgs e)
        {
            client.StartObserve(selectedChats.Select(c => c.Peer), Notify, allChats.Where(c => !selectedChats.Contains(c)).Select(c => c.Peer), cbObserveUnknown.IsChecked.Value);
        }

        private Action<Peer> Notify = (peer) =>
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(config.PathToMusic, UriKind.Absolute));
            player.Play();
        };
    }
}
