using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Entity
{
    public class UpdateTaskModel
    {
        public string TaskNo { get; set; } = string.Empty;
        public string EquipmentType { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string ToAddress { get; set; } = string.Empty;
        public string TaskStatus { get; set; } = string.Empty;
        public string RunStatus { get; set; } = string.Empty;
    }
}
