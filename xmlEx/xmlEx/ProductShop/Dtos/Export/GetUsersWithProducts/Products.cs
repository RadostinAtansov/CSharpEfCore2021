using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.GetUsersWithProducts
{
    [XmlType("Product")]
    public class Products
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}