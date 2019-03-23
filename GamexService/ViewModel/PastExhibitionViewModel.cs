using System;
using System.ComponentModel.DataAnnotations;

namespace GamexService.ViewModel
{
    public class PastExhibitionViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Location")]
        public string Address { get; set; }

        [Display(Name = "From")]
        public DateTime StartDate { get; set; }

        [Display(Name = "To")]
        public DateTime EndDate { get; set; }

        public string Logo { get; set; }

        public string ExhibitionId { get; set; }

        [Display(Name = "Number of attended company")]
        public int CompanyCount { get; set; }

        [Display(Name = "Number of attendee")]
        public int AttendeeCount { get; set; }
    }
}
