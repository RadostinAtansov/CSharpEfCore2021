using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.GetUsersWithProducts
{
    [XmlType("User")]
    public class UserModel
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        public SoldProducts SoldProducts { get; set; }
    }
}