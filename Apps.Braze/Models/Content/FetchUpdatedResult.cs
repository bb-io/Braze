using Apps.Braze.Polling.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Content
{
    public class FetchUpdatedResult
    {
        public DateMemory Memory { get; }
        public List<UpdatedRow> Updated { get; }
        public bool HasUpdates => Updated.Count > 0;

        public FetchUpdatedResult(DateMemory memory, List<UpdatedRow> updated)
        {
            Memory = memory;
            Updated = updated;
        }
    }
    public sealed record UpdatedRow(ContentUpdatedItem Item, DateTime LastEdited);
}
