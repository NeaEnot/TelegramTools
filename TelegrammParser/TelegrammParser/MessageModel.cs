using System.Collections.Generic;
using TL;

namespace TelegrammParser
{
    internal class MessageModel
    {
        public Message TlMessage { get; private set; }
        public UserModel User { get; private set; }
        public UserModel FwdUser { get; private set; }

        public MessageModel(Message message, Dictionary<long, UserModel> users)
        {
            TlMessage = message;

            if (message.From != null)
                User = users[message.From.ID];
            else if (message.Peer != null && users.ContainsKey(message.Peer.ID))
                User = users[message.Peer.ID];

            if (message.fwd_from != null && message.fwd_from.from_id != null && users.ContainsKey(message.fwd_from.from_id.ID))
                FwdUser = users[message.fwd_from.from_id.ID];
        }

        public override string ToString()
        {
            string date = TlMessage.Date.ToString("dd.MM.yyyy - HH:mm:ss");
            string from = User.Name;
            string fwdFrom = FwdUser == null ? "" : $" <-- {FwdUser.Name}";
            string text = TlMessage.message;

            return $"{from}{fwdFrom}\n{date}\n\n{text}\n\n----------------------------------------------------------------\n\n";
        }
    }
}
