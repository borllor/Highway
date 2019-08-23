using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;


namespace JinRi.Notify.Business
{
    public class RedoFacade
    {
        private static readonly RedoMessageBusiness m_redoBus = new RedoMessageBusiness();

        public void Redo()
        {
            m_redoBus.Scan();
        }
    }
}
