using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.GetUsersWithProducts
{
    [XmlType("Users")]
    public class UsersCount
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public List<UserModel> UserOutput { get; set; }
    }
}