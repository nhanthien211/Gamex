using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace GamexService.Utilities
{
    public class FileImageAttribute : ValidationAttribute
    {
        private static readonly string JpegSignature = "FF-D8-FF";
        private static readonly string PngSignature = "89-50-4E-47-0D-0A-1A-0A";

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null || file.ContentLength > 10 * 1024 * 1024)
            {
                return false;
            }
            return IsImageFile(file);
        }

        private bool IsImageFile(HttpPostedFileBase file)
        {
            byte[] bytes = ConvertFileToByteArray(file);
            if (bytes.Length < 8)
            {
                return false;
            }
            string signature = GetFileSignature(bytes);
            file.InputStream.Position = 0;
            return signature.Contains(JpegSignature)                    
                   || signature.Contains(PngSignature);
        }

        private string GetFileSignature(byte[] bytes)
        {
            var signatureByte = new byte[8];
            Array.Copy(bytes, signatureByte, signatureByte.Length);
            return BitConverter.ToString(signatureByte);
        }

        private byte[] ConvertFileToByteArray(HttpPostedFileBase file)
        {
            var array = new Byte[file.ContentLength];
            file.InputStream.Position = 0;
            file.InputStream.Read(array, 0, file.ContentLength);
            return array;
        }
    }
}
