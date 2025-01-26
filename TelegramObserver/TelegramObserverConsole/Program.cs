using Newtonsoft.Json;
using TelegramLibrary;
using TL;

namespace TelegramObserverConsole
{
    internal class Program
    {
        private static TelegramClient client;
        private static Dictionary<long, ChatModel> chatsDict;
        private static Dictionary<long, ChatModel> selectedChatsDict;

        static void Main(string[] args)
        {
            selectedChatsDict = new Dictionary<long, ChatModel>();
            Init().Wait();

            while (true)
            {
                try
                {
                    string input = Console.ReadLine();
                    string cmd = input.Split(' ')[0];

                    switch (cmd)
                    {
                        case "help": Help(); break;
                        case "getchats": GetChats(); break;
                        case "select": SelectChat(long.Parse(input.Split(' ')[1])); break;
                        case "getselected": WriteChats(selectedChatsDict.Values); break;
                        case "startobserve": StartObserve(); break;
                        case "save": Save(); break;
                        case "load": Load(); break;
                        default: Console.WriteLine("you can write help"); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static async Task Init()
        {
            Console.Write("Enter api id: ");
            int apiId = int.Parse(Console.ReadLine());
            Console.Write("Enter api hash: ");
            string apiHash = Console.ReadLine();
            Console.Write("Enter phone number: ");
            string phoneNumber = Console.ReadLine();

            client = new TelegramClient(apiId, apiHash, phoneNumber);
            await client.Connect(AskCode);
        }

        private static void Help()
        {
            Console.WriteLine();
            Console.WriteLine("help");
            Console.WriteLine("getchats");
            Console.WriteLine("select 'id'");
            Console.WriteLine("getselected");
            Console.WriteLine("startobserve");
            Console.WriteLine("save");
            Console.WriteLine("load");
            Console.WriteLine();
        }

        private static async Task GetChats()
        {
            IEnumerable<ChatModel> chats = await client.GetChats();
            chatsDict = chats.ToDictionary(c => c.Peer.ID);
            WriteChats(chats);
        }

        private static async void SelectChat(long id)
        {
            if (chatsDict.ContainsKey(id))
                selectedChatsDict[id] = chatsDict[id];
        }

        private static async Task StartObserve()
        {
            client.StartObserve(selectedChatsDict.Values.Select(c => c.Peer), Notify);
        }

        private static void Save()
        {
            using (StreamWriter writer = new StreamWriter("peers.json"))
            {
                string json = JsonConvert.SerializeObject(selectedChatsDict);
                writer.Write(json);
            }
        }

        private static void Load()
        {
            using (StreamReader reader = new StreamReader("peers.json"))
            {
                string json = reader.ReadToEnd();
                Dictionary<long, ChatModel> restored = JsonConvert.DeserializeObject<Dictionary<long, ChatModel>>(json);

                selectedChatsDict = restored ?? new Dictionary<long, ChatModel>();
            }
        }

        private static void WriteChats(IEnumerable<ChatModel> chats)
        {
            Console.WriteLine();
            foreach (ChatModel chat in chats)
                Console.WriteLine($"{chat.Peer.ID}\t:\t{chat.Title}");
            Console.WriteLine();
        }

        private static Func<string> AskCode = () =>
        {
            Console.WriteLine();
            Console.Write("Enter the code: ");
            string code = Console.ReadLine();
            Console.WriteLine();
            return code;
        };

        private static Action<Peer> Notify = (peer) =>
        {

        };
    }
}