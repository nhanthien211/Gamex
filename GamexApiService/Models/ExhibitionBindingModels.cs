using System.ComponentModel.DataAnnotations;

namespace GamexApiService.Models {
    public class ExhibitionCheckInBindingModel {
        [Required]
        public string Id { get; set; }
    }
}