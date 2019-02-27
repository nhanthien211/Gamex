using Firebase.Storage;
using System.IO;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GamexWeb.Utilities
{
    public static class FirebaseUploadUtility
    {
        private static readonly string FirebaseBucket = WebConfigurationManager.AppSettings.Get("FirebaseBucket");
        private static readonly string ImagePath = "Image";
        private static readonly string ExhibitionCoverPath = "ExhibitionCover";

        public static async Task<string> UploadImageToFirebase(Stream stream, string fileName)
        {
            var task = new FirebaseStorage(FirebaseBucket, new FirebaseStorageOptions
                {
                    ThrowOnCancel = false
                })
                .Child(ImagePath)
                .Child(ExhibitionCoverPath)
                .Child(fileName)
                .PutAsync(stream);
            var url = "";
            try
            {
                url = await task;
            }
            catch (FirebaseStorageException)
            {
                return null;
            }
            return url;
        }        
    }
}