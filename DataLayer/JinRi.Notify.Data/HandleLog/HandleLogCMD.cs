using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Flight.Booking.DB
{
    internal class HandleLogCMD
    {
        public static readonly HandleLogCMD Instance = new HandleLogCMD();
      
    }
}
