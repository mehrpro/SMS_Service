//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMS_Service
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClassRoom
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClassRoom()
        {
            this.Registereds = new HashSet<Registered>();
        }
    
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int ClassLevel_FK { get; set; }
        public System.DateTime ClassRegisterDate { get; set; }
        public bool ClassRoomEnable { get; set; }
    
        public virtual ClassLevel ClassLevel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Registered> Registereds { get; set; }
    }
}
