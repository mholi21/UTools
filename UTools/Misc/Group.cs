using System.Collections.Generic;

namespace UTools.Misc
{
    public class Group
    {
        public string Name;

        public List<Message> Messages;

        public Group(string name, List<Message> Messages)
        {
            Name = name;
            this.Messages = Messages;
        }

        public Group()
        {
        }
    }
}
