using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSReportBarcode
{
    public class cReportBarcode
    {

        public String encodeTo128(String dataToEncode) {
            return code128b(dataToEncode);
        }

        public String code128a(String dataToEncode) {
            String printableString = char.ConvertFromUtf32(203);
            int weightedTotal = 103;
            int currentValue = 0;
            int currentCharNum;
            String c128CheckDigit = "";
            char[] charData = dataToEncode.ToCharArray();
            int stringLength = dataToEncode.Length;

            for (int i = 0; i < stringLength; i++) {

                currentCharNum = (int)charData[i];

                if (currentCharNum < 135) currentValue = currentCharNum - 32;
                if (currentCharNum > 134) currentValue = currentCharNum - 100;

                currentValue *= i;
                weightedTotal += currentValue;

                if (currentCharNum == 32) currentCharNum = 194;

                printableString += char.ConvertFromUtf32(currentCharNum);
            }

            int checkDigitValue = weightedTotal % 103;

            if (checkDigitValue < 95 && checkDigitValue > 0) c128CheckDigit = char.ConvertFromUtf32(checkDigitValue + 32);
            if (checkDigitValue > 94) c128CheckDigit = char.ConvertFromUtf32(checkDigitValue + 100);
            if (checkDigitValue == 0) c128CheckDigit = char.ConvertFromUtf32(194);

            return printableString + c128CheckDigit + char.ConvertFromUtf32(206);
        }

        private bool isNumeric(string value)
        {
            double dummyNumber;
            return Double.TryParse(value, out dummyNumber);
        }

        public String code128c(String dataToEncode, int returnType = 0) {
            int currentValue;
            String c128CheckDigit = "";
            int stringLength = dataToEncode.Length; ;
            String onlyCorrectData = "";

            // additional logic needed in case ReturnType is not entered

            if (returnType != 0 && returnType != 1 && returnType != 2) returnType = 0;

            for (int i = 0; i < stringLength; i++) {

                if (isNumeric(dataToEncode.Substring(i, 1))) onlyCorrectData = onlyCorrectData + dataToEncode.Substring(i, 1);

            }

            dataToEncode = onlyCorrectData;

            if (dataToEncode.Length % 2 == 1) dataToEncode = "0" + dataToEncode;

            String printableString = char.ConvertFromUtf32(205);

            int weightedTotal = 105;
            int weightValue = 1;

            stringLength = dataToEncode.Length;

            for (int i = 1; i < stringLength; i += 2) {

                currentValue = int.Parse(dataToEncode.Substring(i, 2));

                if (currentValue < 95 && currentValue > 0) printableString += char.ConvertFromUtf32(currentValue + 32);
                if (currentValue > 94) printableString += char.ConvertFromUtf32(currentValue + 100);
                if (currentValue == 0) printableString += char.ConvertFromUtf32(194);

                currentValue *= weightValue;
                weightedTotal += currentValue;
                weightValue += 1;
            }

            int checkDigitValue = weightedTotal % 103;

            if (checkDigitValue < 95 && checkDigitValue > 0) c128CheckDigit = char.ConvertFromUtf32(checkDigitValue + 32);

            if (checkDigitValue > 94) c128CheckDigit = char.ConvertFromUtf32(checkDigitValue + 100);

            if (checkDigitValue == 0) c128CheckDigit = char.ConvertFromUtf32(194);

            if (returnType == 0) return printableString + c128CheckDigit + char.ConvertFromUtf32(206);
            if (returnType == 1) return dataToEncode + checkDigitValue;
            /* returnType == 2*/ return checkDigitValue.ToString();
        }

        public String code128b(String dataToEncode)
        {
            int currentValue = 0;
            int currentCharNum;
            int checkDigitValue;
            String c128CheckDigit = "";

            int weightedTotal = 104;
            String printableString = char.ConvertFromUtf32(204);
            char[] charData = dataToEncode.ToCharArray();
            int stringLength = dataToEncode.Length;

            for (int i = 0; i < stringLength; i++)
            {

                currentCharNum = (int)charData[i];
                if (currentCharNum < 135) currentValue = currentCharNum - 32;
                if (currentCharNum > 134) currentValue = currentCharNum - 100;
                currentValue *= i;
                weightedTotal += currentValue;

                if (currentCharNum == 32) currentCharNum = 194;

                printableString += char.ConvertFromUtf32(currentCharNum);
            }

            checkDigitValue = weightedTotal % 103;
            if (checkDigitValue < 95 && checkDigitValue > 0) c128CheckDigit = char.ConvertFromUtf32(checkDigitValue + 32);
            if (checkDigitValue > 94) c128CheckDigit = char.ConvertFromUtf32(checkDigitValue + 100);
            if (checkDigitValue == 0) c128CheckDigit = char.ConvertFromUtf32(194);

            return printableString + c128CheckDigit + char.ConvertFromUtf32(206);
        }
    }
}
