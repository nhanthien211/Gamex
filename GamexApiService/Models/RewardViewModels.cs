namespace GamexApiService.Models {
    public class RewardDetailViewModel {
        public int RewardId { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public int Quantity { get; set; }
        public int PointCost { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class RewardContentViewModel {
        public int RewardId { get; set; }
        public string Content { get; set; }
        public int PointCost { get; set; }
    }

    public class RewardShortViewModel {
        public int RewardId { get; set; }
        public int PointCost { get; set; }
        public string Content { get; set; }
    }

    public class RewardPointViewModel {
        public int Point { get; set; }
        public int TotalPointEarned { get; set; }
    }
}