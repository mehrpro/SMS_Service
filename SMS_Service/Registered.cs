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
    
    public partial class Registered
    {
        public int ID { get; set; }
        public int Years_FK { get; set; }
        public int School_FK { get; set; }
        public int Class_FK { get; set; }
        public int Student_FK { get; set; }
        public bool Enabled { get; set; }
    
        public virtual AcademicYear AcademicYear { get; set; }
        public virtual ClassRoom ClassRoom { get; set; }
        public virtual School School { get; set; }
        public virtual Student Student { get; set; }
    }
}