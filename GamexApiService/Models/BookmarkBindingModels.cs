using System.ComponentModel.DataAnnotations;

namespace GamexApiService.Models {
    public class BookmarkBindingModel {
        [Required] public string Id { get; set; }
    }
}