using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class CommandLog
    {
        public int Id { get; set; }
        public string CommandType { get; set; }
        public string CommandData { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
