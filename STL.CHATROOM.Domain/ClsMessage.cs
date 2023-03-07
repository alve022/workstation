using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STL.CHATROOM.Domain.ENUM_LIST;
using static System.Collections.Specialized.BitVector32;

namespace STL.CHATROOM.Domain
{
    public class ClsMessage
    {
        public string FROM { get; set; }
        public string TO { get; set; }
        public string BODY { get; set; }
        public string USERNAME { get; set; }
        public string IMAGE { get; set; }
        public List<OnlineUser>? Users { get; set; }
        public ACTION ACTION { get; set; }
    }
}
