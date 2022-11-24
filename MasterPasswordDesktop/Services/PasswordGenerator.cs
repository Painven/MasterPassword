using MasterPasswordDesktop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterPasswordDesktop.Infrastructure
{
    public static class PasswordGenerator
    {
        static Random random = new Random();
   
        internal static string Generate(int passwordLength, PasswordGeneratorOptions options)
        {
            string result = string.Empty;

            if (options.HasFlag(PasswordGeneratorOptions.Guid))
            {
                result = Guid.NewGuid().ToString();
                if(options.HasFlag(PasswordGeneratorOptions.UpperCase))
                {
                    result = result.ToUpper();
                }
            }
            else
            {
                var sb = new StringBuilder();

                List<char> validCharList = GetValidCharList(options);

                if(validCharList.Count == 0)
                {
                    return string.Empty;
                }

                bool useLowerCase = options.HasFlag(PasswordGeneratorOptions.LowerCase);
                for (int i = 0; i < passwordLength; i++)
                {
                    char nextChar = validCharList[random.Next(validCharList.Count)];
                   
                    if(options.HasFlag(PasswordGeneratorOptions.UpperCase))
                    {
                        if(options.HasFlag(PasswordGeneratorOptions.LowerCase))
                        {
                            bool isUpper = random.Next(2) == 1;
                            if(isUpper)
                            {
                                nextChar = char.ToUpper(nextChar); 
                            }
                        }
                        else
                        {
                            nextChar = char.ToUpper(nextChar);
                        }
                    }
                       
                    sb.Append(nextChar);
                }

                result = sb.ToString();
                
            }

            return result;
        }

        private static List<char> GetValidCharList(PasswordGeneratorOptions options)
        {
            //var invalidLetters = new char[] { '1', '0', 'о', 'О', 'o', 'O', 'l', 'i', 'I', 'L' };

            var letters_a_z = Enumerable.Range('a', 'z' - 'a' + 1).Select(i => (Char)i).ToArray();
            var letters_а_я = Enumerable.Range('а', 'я' - 'а' + 1).Select(i => (Char)i).ToArray();
            var digits = "0123456789".ToArray();
            var specialSymbols = "!@#$%^&*()".ToArray();

            List<char> validChars = new List<char>();

            if (options.HasFlag(PasswordGeneratorOptions.EnglishLetters))
            {
                validChars = validChars.Concat(letters_a_z).ToList();
            }
            if (options.HasFlag(PasswordGeneratorOptions.RussianLetters))
            {
                validChars = validChars.Concat(letters_а_я).ToList();
            }
            if (options.HasFlag(PasswordGeneratorOptions.SpecialSymbols))
            {
                validChars = validChars.Concat(specialSymbols).ToList();
            }
            if (options.HasFlag(PasswordGeneratorOptions.Digits))
            {
                validChars = validChars.Concat(digits).ToList();
            }

            return validChars;
            //return validChars.Except(invalidLetters).ToArray();
        }
    }
}
