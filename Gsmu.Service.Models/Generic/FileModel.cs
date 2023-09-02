using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.Models.Generic
{
    public class FileModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public string Directory { get; set; }
        public float Size { get; set; } 
        public DateTime LastDateModified { get; set; }
    }
}
