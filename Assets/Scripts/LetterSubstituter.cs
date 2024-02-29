public class LetterSubstituter
{
    public static void SubstituteLetterToNumber(char letter, char nextLetter, ref string modifiedWord)
    {
        switch (letter)
        {
            case 'C':
                if (nextLetter == 'S')
                {
                    modifiedWord = modifiedWord.Replace("CS", "0");
                }

                break;
            case 'G':
                if (nextLetter == 'Y')
                {
                    modifiedWord = modifiedWord.Replace("GY", "1");
                }

                break;
            case 'L':
                if (nextLetter == 'Y')
                {
                    modifiedWord = modifiedWord.Replace("LY", "2");
                }

                break;

            case 'N':
                if (nextLetter == 'Y')
                {
                    modifiedWord = modifiedWord.Replace("NY", "3");
                }

                break;
            case 'S':
                if (nextLetter == 'Z')
                {
                    modifiedWord = modifiedWord.Replace("SZ", "4");
                }

                break;
            case 'T':
                if (nextLetter == 'Y')
                {
                    modifiedWord = modifiedWord.Replace("TY", "5");
                }

                break;
            case 'Z':
                if (nextLetter == 'S')
                {
                    modifiedWord = modifiedWord.Replace("ZS", "6");
                }

                break;
        }
    }

    public static void SubstituteNumberToLetter(char number, ref string modifiedWord)
    {
        switch (number)
        {
            case '0':
                modifiedWord = modifiedWord.Replace("0", "CS");
                break;
            case '1':
                modifiedWord = modifiedWord.Replace("1", "GY");
                break;
            case '2':
                modifiedWord = modifiedWord.Replace("2", "LY");
                break;
            case '3':
                modifiedWord = modifiedWord.Replace("3", "NY");
                break;
            case '4':
                modifiedWord = modifiedWord.Replace("4", "SZ");
                break;
            case '5':
                modifiedWord = modifiedWord.Replace("5", "TY");
                break;
            case '6':
                modifiedWord = modifiedWord.Replace("6", "ZS");
                break;
        }
    }
}