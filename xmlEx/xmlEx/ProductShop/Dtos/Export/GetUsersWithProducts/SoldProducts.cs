using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.GetUsersWithProducts
{
    [XmlType("SoldProducts")]
    public class SoldProducts
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public List<Products> Products { get; set; }
    }
}