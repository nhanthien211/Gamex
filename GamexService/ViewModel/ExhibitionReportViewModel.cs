using System;
using System.Collections.Generic;

namespace GamexService.ViewModel
{
    public class ExhibitionReportViewModel
    {
        public List<CompanyReport> CompanyReport { get; set; }
        public List<AttendeeReport> AttendeeReport { get; set; }
    }

    public class CompanyReport
    {
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
    }

    public class AttendeeReport
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CheckinTime { get; set; }
    }
}
