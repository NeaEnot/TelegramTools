using TL;
using WTelegram;

namespace TelegramLibrary
{
    public class TelegramClient
    {
        private int apiId;
        private string apiHash;
        private string phoneNumber;

        private Client client;
        private Func<string> codeRequestFunc { get; set; }

        public TelegramClient(int apiId, string apiHash, string phoneNumber)
        {
            this.apiId = apiId;
            this.apiHash = apiHash;
            this.phoneNumber = phoneNumber;
        }

        public async Task Connect(Func<string> codeRequestFunc = null)
        {
            this.phoneNumber = phoneNumber;
            this.codeRequestFunc = codeRequestFunc ?? this.codeRequestFunc;

            client = new Client(Config);
            Environment.SetEnvironmentVariable("lvl", "6");
            WTelegram.Helpers.Log = (lvl, s) => System.Diagnostics.Debug.WriteLine(s);
            await client.LoginUserIfNeeded();
        }

        private string Config(string what)
        {
            switch (what)
            {
                case "api_id": return apiId.ToString();
                case "api_hash": return apiHash;
                case "phone_number": return phoneNumber;
                case "verification_code": return codeRequestFunc();
                case "session_pathname": return "WTelegram.session";
                //case "first_name": return "John";
                //case "last_name": return "Doe";
                //case "password": return "secret!";
                default: return null;
            }
        }

        public async Task<IEnumerable<ChatModel>> GetChats()
        {
            Messages_Dialogs dialogs = await client.Messages_GetAllDialogs();
            DialogsParser dialogsParser = new DialogsParser();
            
            return dialogsParser.Parse(dialogs);
        }

        public async void StartObserve(IEnumerable<Peer> peers, Action<Peer> notify, IEnumerable<Peer> notObservedPeers = null, bool observeUnknown = false)
        {
            if (notObservedPeers == null)
                notObservedPeers = new List<Peer>();

            client.OnUpdates += async (updates) =>
            {
                if (updates is UpdatesBase updatesBase)
                {
                    foreach (var update in updatesBase.UpdateList)
                    {
                        if (update is UpdateNewMessage updateNewMessage)
                        {
                            var message = updateNewMessage.message;
                            if (peers.Count(p => p.ID == message.Peer.ID) > 0 || (observeUnknown && notObservedPeers.Count(p => p.ID == message.Peer.ID) == 0))
                                notify(message.Peer);
                        }
                    }
                }
            };
        }
    }
}
