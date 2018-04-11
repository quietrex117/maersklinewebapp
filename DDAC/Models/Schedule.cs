//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DDAC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Schedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Schedule()
        {
            this.ScheduleBookings = new HashSet<ScheduleBooking>();
        }
    
        public int ScheduleId { get; set; }
        
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> ShipmentDate { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public Nullable<double> Rate { get; set; }
        public int EarliestDeparture { get; set; }
        public bool Status { get; set; }
        public Nullable<int> VesselsID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScheduleBooking> ScheduleBookings { get; set; }
    }
}