using System.Collections.Generic;
using System.Linq;

namespace MaginusLunch.Core.Validation
{
    public class ValidationStatus
    {
        private readonly IList<Reason> _reasons = new List<Reason>();

        public virtual bool IsValid => !Reasons.Any(r => r.Code != Reason.OK);

        public IEnumerable<Reason> Reasons => _reasons;

        public void AddReason(Reason reason)
        {
            _reasons.Add(reason);
        }

        public class Reason
        {
            public const int OK = 200;
            public Reason(int code, string text)
            {
                Text = text;
                Code = code;
            }
            public string Text { get; }
            public int Code { get; }
        }
    }
}
