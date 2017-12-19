using System.Xml.Serialization;

namespace UTools.Misc
{
    public sealed class Message
    {
        [XmlAttribute("Text")]
        public string text;

        [XmlAttribute("Color")]
        public string color;

        public Message(string text, string color)
        {
            this.text = text;
            this.color = color;
        }

        public Message()
        {
            text = string.Empty;
            color = string.Empty;
        }
    }
}
