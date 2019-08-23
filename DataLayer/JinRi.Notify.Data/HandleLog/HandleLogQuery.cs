using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;


namespace Flight.Booking.DB
{
    internal class HandleLogQuery
    {
        public static readonly HandleLogQuery Instance = new HandleLogQuery();

    }
}
