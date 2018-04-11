using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DDAC.Models
{
    [MetadataType(typeof(BookingMetadata))]
    public partial class Booking
    {

    }

    public class BookingMetadata
    {
        [Required (AllowEmptyStrings = false, ErrorMessage = "Please Provide Cargo")]
        public String Cargo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Container Type")]
        public String ContainerType { get; set; }
    }
}