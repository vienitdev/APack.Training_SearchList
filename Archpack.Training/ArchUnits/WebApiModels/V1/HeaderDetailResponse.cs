using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiModels.V1
{
    public class HeaderDetailResponse<THead, TDetail>
    {

        public THead Header { get; set; }
        public IEnumerable<TDetail> Details { get; set; }

    }
}