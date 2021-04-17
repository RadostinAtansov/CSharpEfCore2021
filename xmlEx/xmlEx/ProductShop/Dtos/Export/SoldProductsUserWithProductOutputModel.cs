using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("SoldProducts")]
    public class SoldProductsUserWithProductOutputModel
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

    }
}