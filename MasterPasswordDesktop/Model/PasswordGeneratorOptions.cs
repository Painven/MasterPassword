using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterPasswordDesktop.Model
{
    [Flags]
    public enum PasswordGeneratorOptions
    {
        None = 0,
        Guid = 1,
        EnglishLetters = 2,
        RussianLetters = 4,
        LowerCase = 8,
        Digits = 16,
        UpperCase = 32,
        SpecialSymbols = 64
    }
}
