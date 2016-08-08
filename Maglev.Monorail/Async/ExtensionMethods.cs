using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Async
{
    public static class ExtensionMethods
    {
        public static AsyncNode WriteLine(this AsyncNode input, string value)
        {
            var rv = new AsyncNode(() => Console.WriteLine(value)) {Status = AsyncStatus.Queued};

            return input.Then(rv);
        }
    }
}
