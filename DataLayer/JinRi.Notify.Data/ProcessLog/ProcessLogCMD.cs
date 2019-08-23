using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Flight.Booking.DB
{
    internal class ProcessLogCMD
    {
        public static readonly ProcessLogCMD Instance = new ProcessLogCMD();
      
    }
}
