//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GamexEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class ActivityHistory
    {
        public int ActivityId { get; set; }
        public string AccountId { get; set; }
        public string Activity { get; set; }
        public System.DateTime Time { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
