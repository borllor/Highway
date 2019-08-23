using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JinRi.Notify.Business.Core;

namespace JinRi.Notify.Business
{
  public  class ScanFacade
    {
        ScanMessageBusiness _scanBus = new ScanMessageBusiness();

        public void Scan()
      {
          _scanBus.Scan();
      }
    }
}
