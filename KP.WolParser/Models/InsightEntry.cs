using System;
using System.Collections.Generic;
using System.Text;

namespace KP.WolParser.Models
{
    public class InsightEntry : BaseWolEntry
    {
        public string Content { get; set; }

        public int SizeInPages { get; set; }
        public long SizeInChars
        {
            get
            {
                if (Content?.Length != null) return (long)Content?.Length;
                return 0;
            }
        }

        private string _page;
        public string Page
        {
            get
            {
                return _page;
            }
            set {
                _page = value;
                
                var pages= _page.Split('-');
                int.TryParse(pages[0], out int min);
                int max = min;
                if (pages.Length == 2)
                    int.TryParse(pages[1], out  max);
                SizeInPages = (max - min) + 1;
            }
        }
    }
}
