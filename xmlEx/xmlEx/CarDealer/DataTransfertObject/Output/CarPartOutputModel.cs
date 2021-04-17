using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransfertObject.Output
{
    [XmlType("car")]
    public class CarPartOutputModel
    {

        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravekkedDistance { get; set; }

        [XmlArray("parts")]
        public CarPartInfoOutputModel[] Parts { get; set; }

        
    }

}
