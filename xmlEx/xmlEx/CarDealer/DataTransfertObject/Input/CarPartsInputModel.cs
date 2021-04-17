using System.Xml.Serialization;

namespace CarDealer.DataTransfertObject.Input
{
    [XmlType("partId")]
    public class CarPartsInputModel
    {

        [XmlAttribute("id")]

        public int Id { get; set; }
    }
}