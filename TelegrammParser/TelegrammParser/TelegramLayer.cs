using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TL;
using WTelegram;

namespace TelegrammParser
{
    internal class TelegramLayer
    {
        private static int apiId = 0;
        private static string apiHash = "";

        private string phoneNumber;

        private Client client;

        public async Task Connect(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;

            client = new Client(Config);
            await client.LoginUserIfNeeded();
        }

        private string Config(string what)
        {
            switch (what)
            {
                case "api_id": return apiId.ToString();
                case "api_hash": return apiHash;
                case "phone_number": return phoneNumber;
                case "verification_code": return GetCode();
                //case "first_name": return "John";
                //case "last_name": return "Doe";
                //case "password": return "secret!";
                default: return null;
            }
        }

        private string GetCode()
        {
            string code = Application.Current.Dispatcher.Invoke(() =>
            {
                CodeWindow window = new CodeWindow();

                if (window.ShowDialog() == true)
                    return window.Code;

                return "";
            });

            return code;
        }

        public async Task<IEnumerable<UserModel>> GetChats()
        {
            Messages_Dialogs dialogs = await client.Messages_GetAllDialogs();
            IEnumerable<User> users = dialogs.users.Values;
            IEnumerable<UserModel> result = users.Select(u => new UserModel(u));

            return result;
        }

        public async Task<List<MessageModel>> DownloadChat(UserModel user, DateTime startDate, DateTime endDate)
        {
            List<Message> messagesResponses = new List<Message>();
            int offsetId = 0;

            InputPeer userPeer = new InputPeerUser(user.TlUser.id, user.TlUser.access_hash);

            UserModel currentUser = new UserModel(client.User);
            Dictionary<long, UserModel> users = new Dictionary<long, UserModel>();
            users.Add(currentUser.TlUser.ID, currentUser);
            users.Add(user.TlUser.ID, user);

            while (true)
            {
                var history = await client.Messages_GetHistory(userPeer, offset_id: offsetId, limit: 100);

                var msgs = history.Messages.OfType<Message>()
                    .Where(m => m.date >= startDate && m.date <= endDate)
                    .ToList();

                if (!msgs.Any())
                    break;

                messagesResponses.AddRange(msgs);
                offsetId = messagesResponses.Last().id;
            }

            IEnumerable<PeerUser> peers =
                messagesResponses
                .Where(m => m.fwd_from != null && m.fwd_from.from_id != null && m.fwd_from.from_id is PeerUser)
                .Select(m => m.fwd_from.from_id)
                .Cast<PeerUser>();

            List<InputUser> inputUsers = new List<InputUser>();

            foreach (PeerUser peer in peers)
            {
                if (users.ContainsKey(peer.ID))
                    continue;

                inputUsers.Add(new InputUser(peer.user_id, 0));
                users.Add(peer.ID, null);
            }

            UserBase[] fwdUsers = await client.Users_GetUsers(inputUsers.ToArray());

            foreach(UserBase fwdUser in fwdUsers)
            {
                if (fwdUser is User)
                    users[fwdUser.ID] = new UserModel(fwdUser as User);
            }

            List<MessageModel> answer = messagesResponses.Where(m => !string.IsNullOrWhiteSpace(m.message)).Select(m => new MessageModel(m, users)).ToList();

            return answer;
        }
    }
}
