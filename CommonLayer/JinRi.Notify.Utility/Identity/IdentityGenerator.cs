using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.Utility
{
    public class IdentityGenerator
    {
        public static string New()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
