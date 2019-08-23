using System;
using System.Collections.Generic;

using JinRi.Notify.Entity;

namespace Flight.Booking.DB
{
    public class ProcessLogFacade
    {
        public static readonly ProcessLogFacade Instance = new ProcessLogFacade();

        public int Add(ProcessLogEntity entity)
        {
            return 0;
        }
    }
}