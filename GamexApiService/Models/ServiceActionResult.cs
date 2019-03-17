using GamexEntity;

namespace GamexApiService.Models {
    public class ServiceActionResult {
        public bool Ok { get; set; }
        public string Message { get; set; }
        public static readonly ServiceActionResult ErrorResult = new ServiceActionResult {
            Ok = false,
            Message = "Oops, something went wrong!"
        };
    }

    public class BookmarkServiceActionResult : ServiceActionResult {
        public AspNetUsers TgtAccount { get; set; }
        public Company Company { get; set; }
        public Exhibition Exhibition { get; set; }
    }
}