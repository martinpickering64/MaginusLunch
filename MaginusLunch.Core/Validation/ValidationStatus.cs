using System.Collections.Generic;
using System.Linq;

namespace MaginusLunch.Core.Validation
{
    public class ValidationStatus
    {
        public ValidationStatus()
        {
            Reasons = new List<Reason>();
        }
        public virtual bool IsValid => !Reasons.Any() || !Reasons.Any(r => r.Code != Reason.OK);

        public virtual IList<Reason> Reasons { get; }

        public class Reason
        {
            public const int OK = 200;
            public Reason (int code, string text)
            {
                Text = text;
                Code = code;
            }
            public string Text { get; }
            public int Code { get; }
        }
    }
}
