namespace GamexApiService.Models {
    public class CompanyShortViewModel {
        public string CompanyId { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string[] Booths { get; set; }
    }

    public class CompanyViewModel {
        public string CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string TaxNumber { get; set; }
        public bool IsBookmarked { get; set; }
    }
}