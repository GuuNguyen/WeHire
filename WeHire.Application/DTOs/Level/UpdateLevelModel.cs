using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.Level
{
    public class UpdateLevelModel
    {
        [Required(ErrorMessage = "LevelId is required")]
        public int LevelId { get; set; }
        [Required(ErrorMessage = "LevelName is required")]
        public string LevelName { get; set; }
        [Required(ErrorMessage = "LevelDescription is required")]
        public string LevelDescription { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public int Status { get; set; }
    }
}
