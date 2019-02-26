using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GamexApi.Models {

    // Models returned by ExhibitionController actions

    public class ExhibitionViewModel {
        public int ExhibitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string OrganizerId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Location { get; set; }
        public string Logo { get; set; }
        public string Qr { get; set; }
    }
}