using System.ComponentModel.DataAnnotations;
using Villa_Web.Models.Dto.Villa;

namespace Villa_Web.Models.Dto.VillaNumber
{
    public class VillaNumberDTO
    {
        [Required]
        public int VillaNo { get; set; }
        [Required]
        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }

        // Villa Navigation Property ...
        public VillaDTO Villa { get; set; }
    }
}
