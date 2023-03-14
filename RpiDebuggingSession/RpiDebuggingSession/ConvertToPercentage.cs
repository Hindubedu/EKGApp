using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiDebuggingSession
{
    public class ConvertToPercentage
    {
        public int ConvertTo100(short value)
        {
            var result = Convert.ToDouble(value) / 4096.0 * 100.0;

            return (int)result;
        }
    }
}
