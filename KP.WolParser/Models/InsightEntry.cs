using System;
using System.Collections.Generic;
using System.Text;

namespace KP.WolParser.Models
{
    public class InsightEntry : BaseWolEntry
    {
        public string Content { get; set; }

        public long Size
        {
            get
            {
                if (Content?.Length != null) return (long) Content?.Length;
                return 0;
            }
        }

        public string Page { get; set; }
    }
}
