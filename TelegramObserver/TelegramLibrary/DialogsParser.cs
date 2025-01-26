using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;

namespace TelegramLibrary
{
    internal class DialogsParser
    {
        public List<ChatModel> Parse(Messages_Dialogs messagesDialogs)
        {
            List<ChatModel> chats = new List<ChatModel>();

            foreach (var dialog in messagesDialogs.Dialogs)
            {
                Peer peer = dialog.Peer;
                string title = null;

                if (peer is PeerUser)
                    title = GetUserName(messagesDialogs.users[peer.ID]);
                if (peer is PeerChat || peer is PeerChannel)
                    title = messagesDialogs.chats[peer.ID].Title;

                chats.Add(new ChatModel
                {
                    Peer = peer,
                    Title = title
                });
            }

            return chats;
        }

        private string GetUserName(User user)
        {
            string result = "";

            if (!string.IsNullOrEmpty(user.first_name))
                result += user.first_name;

            if (!string.IsNullOrEmpty(user.last_name))
                result += string.IsNullOrEmpty(result) ? user.last_name : (" " + user.last_name);

            if (!string.IsNullOrEmpty(user.username))
            {
                if (string.IsNullOrEmpty(result))
                    result = user.username;
                else
                    result += " (" + user.username + ")";
            }

            return result;
        }
    }
}
