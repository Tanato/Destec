using Destec.CoreApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
