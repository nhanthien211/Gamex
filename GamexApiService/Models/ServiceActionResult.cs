namespace GamexApiService.Models {
    public class ServiceActionResult {
        public bool Ok { get; set; }
        public string Message { get; set; }
        public static readonly ServiceActionResult ErrorResult = new ServiceActionResult {
            Ok = false,
            Message = "Oops, something went wrong!"
        };
    }
}