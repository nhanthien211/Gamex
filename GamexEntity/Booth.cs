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
    
    public partial class Booth
    {
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string ExhibitionId { get; set; }
        public string BoothNumber { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual Exhibition Exhibition { get; set; }
    }
}
