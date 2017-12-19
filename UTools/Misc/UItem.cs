using System.Xml.Serialization;

namespace UTools.Misc
{
    public class UItem
    {
        private UItem()
        {
        }

        public UItem(ushort id, decimal? money)
        {
            ID = id;
            Money = money;
        }

        [XmlAttribute("id")]
        public ushort ID;

        [XmlAttribute("amount")]
        public decimal? Money;
    }
}
