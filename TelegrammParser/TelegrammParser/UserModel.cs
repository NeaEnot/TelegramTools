using TL;

namespace TelegrammParser
{
    internal class UserModel
    {
        public string Name
        {
            get
            {
                string result = "";

                if (!string.IsNullOrEmpty(TlUser.first_name))
                    result += TlUser.first_name;

                if (!string.IsNullOrEmpty(TlUser.last_name))
                    result += string.IsNullOrEmpty(result) ? TlUser.last_name : (" " + TlUser.last_name);

                if (!string.IsNullOrEmpty(TlUser.username))
                {
                    if (string.IsNullOrEmpty(result))
                        result = TlUser.username;
                    else
                        result += " (" + TlUser.username + ")";
                }

                return result;
            }
        }

        public User TlUser { get; private set; }

        public UserModel(User tlUser)
        {
            TlUser = tlUser;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
