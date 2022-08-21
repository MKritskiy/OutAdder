using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutAdder.resources
{
    internal class TextBoxTemplate
    {
        public TextBoxTemplate(string text)
        {
            CorrectionPos = 0;
            TimeText = text;
        }
        public string TimeText { get; set; }
        private int CorrectionPos { get; set; }
        public void AddTimeNum(string num)
        {
            if (CorrectionPos > 4)
                SubCorrectionPos();
            if (num != null)
            {
                char[] chars = TimeText.ToCharArray();
                chars[CorrectionPos] = Char.Parse(num);
                TimeText = new string(chars);
                AddCorrectionPos();
            }
        }
        public void SubTimeNum()
        {
            char[] chars = TimeText.ToCharArray();
            SubCorrectionPos();
            chars[CorrectionPos] = '0';
            TimeText = new string(chars);
        }
        private void AddCorrectionPos()
        {
            CorrectionPos++;
            if (CorrectionPos == 2)
                CorrectionPos++;
            if (CorrectionPos > 5)
                CorrectionPos--;
        }
        private void SubCorrectionPos()
        {
            CorrectionPos--;
            if (CorrectionPos == 2)
                CorrectionPos--;
            if (CorrectionPos < 0)
                CorrectionPos = 0;
        }
    }
}
