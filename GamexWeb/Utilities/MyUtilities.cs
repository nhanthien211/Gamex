using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GamexWeb.Utilities
{
    public class MyUtilities
    {
        public static string GenerateRandomPassword()
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";
            string special = "!@#$%^";


            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= random.Next(5, 10); i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= random.Next(5, 10); i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= random.Next(5, 10); i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            for (int i = 1; i <= random.Next(5, 10); i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    special[random.Next(special.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);
        }
    }
}