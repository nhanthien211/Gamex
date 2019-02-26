using System.ComponentModel.DataAnnotations;
using System.Web;

namespace GamexService.Utilities
{
    public class FileLength : ValidationAttribute
    {
        public long MaxSize { get; set; }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null || file.ContentLength > MaxSize)
            {
                return false;
            }
            return true;
        }

    }
}
