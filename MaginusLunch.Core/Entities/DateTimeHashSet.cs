using System;
using System.Collections.Generic;

namespace MaginusLunch.Core.Entities
{
    public class DateTimeHashSet : HashSet<DateTime>
    {
        public DateTimeHashSet():base()
        { }

        public DateTimeHashSet(IEnumerable<DateTime> collection)
            :base(collection)
        { }

        public new bool Add(DateTime item)
        {
            if (item.Kind != DateTimeKind.Utc)
            {
                item = item.ToUniversalTime();
            }
            return base.Add(item);
        }
    }
}
