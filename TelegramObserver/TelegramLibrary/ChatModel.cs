using TL;

namespace TelegramLibrary
{
    public class ChatModel
    {
        public Peer Peer { get; set; }
        public string Title { get; set; }

        public override string ToString()
        {
            return Title;
        }

        public override bool Equals(object? obj)
        {
            return obj is ChatModel && (obj as ChatModel).Title == Title;
        }
    }
}
