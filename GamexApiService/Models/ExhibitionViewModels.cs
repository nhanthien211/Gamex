using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GamexApiService.ViewModel {

    // Models returned by ExhibitionController actions

    public class ExhibitionViewModel {
        public string ExhibitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        //public string OrganizerId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Logo { get; set; }
        //public string Lat { get; set; }
        //public string Lng { get; set; }
        public List<CompanyShortViewModel> ListCompany { get; set; }
    }

    public class ExhibitionShortViewModel {
        public string ExhibitionId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Logo { get; set; }
    }
}