using System;
using System.Collections.Generic;

using JinRi.Notify.Entity;

namespace Flight.Booking.DB
{
    public class HandleLogFacade
    {
        public static readonly HandleLogFacade Instance = new HandleLogFacade();

        public int Add(HandleLogEntity entity)
        {
            return 0;
        }
    }
}