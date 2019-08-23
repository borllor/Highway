using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinRi.Notify.ServiceModel.Condition
{
    [Serializable]
    public abstract class BaseCondition
    {
        public virtual int CurPage { get; set; }
        public virtual int PageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public virtual int RecordCount { get; set; }
        public virtual int Status { get; set; }
        public virtual string OrderBy { get; set; }
        public virtual OrderDirectionEnum OrderDirection { get; set; }

        /// <summary>
        /// MySql分页偏移量
        /// </summary>
        public int Offset
        {
            get
            {
                return ((this.CurPage == 0 ? 1 : this.CurPage) - 1) * this.PageSize;
            }
        }

        public int LowerBound
        {
            get
            {
                return ((this.CurPage == 0 ? 1 : this.CurPage) - 1) * this.PageSize + 1;
            }
        }
        public int UpperBound
        {
            get
            {
                return (this.CurPage == 0 ? 1 : this.CurPage) * this.PageSize;
            }
        }

        public BaseCondition()
        {
            CurPage = 1;
            PageSize = 20;
            OrderBy = "CreateTime";
            OrderDirection = OrderDirectionEnum.DESC;
            Status = 2;
        }
    }
}
