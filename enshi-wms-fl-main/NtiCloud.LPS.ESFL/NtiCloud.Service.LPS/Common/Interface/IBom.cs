using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NtiCloud.Service.WMS.Common
{
    public interface IBom
    {
        public DataRow GetHead(string id);
        DataRow[] GetFirstLevelDetail(string bomID);
    }
}
