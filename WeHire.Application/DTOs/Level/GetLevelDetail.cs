using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Level
{
    public class GetLevelDetail
    {
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public string LevelDescription { get; set; }
        public string StatusString { get; set; }
    }
}
