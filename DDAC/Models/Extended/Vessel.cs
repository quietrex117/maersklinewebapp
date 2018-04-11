using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DDAC.Models
{
    [MetadataType(typeof(VesselMetadata))]
    public partial class Vessel
    {

    }

    public class VesselMetadata
    {
        [Required (AllowEmptyStrings = false, ErrorMessage = "Vessel Name Required")]
        public string VesselName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Terminal Required")]
        public string Terminal { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Required")]
        public string Country { get; set; }

    }
}