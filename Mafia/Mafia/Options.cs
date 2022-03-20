using System;
using System.Collections.Generic;
using System.Text;

namespace Mafia
{
    public class Options
    {
        public int DurationOfDiscussion { get; set; }
        public int DurationOfSelectionAtNight { get; set; }
        public Options(string _durOfDisc, string _durOfSel)
        {
            if (_durOfDisc == "")
                DurationOfDiscussion = 0;
            else DurationOfDiscussion = (int)double.Parse(_durOfDisc)*60;
            if (_durOfSel == "")
                DurationOfSelectionAtNight = 0;
            else DurationOfSelectionAtNight = (int)double.Parse(_durOfSel);
            if (DurationOfDiscussion < 0)
                DurationOfDiscussion = 0;
            if (DurationOfSelectionAtNight < 0)
                DurationOfSelectionAtNight = 0;
        }
    }
}
