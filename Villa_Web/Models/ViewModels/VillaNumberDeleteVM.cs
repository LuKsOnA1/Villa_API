using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Villa_Web.Models.Dto.VillaNumber;

namespace Villa_Web.Models.ViewModels
{
	public class VillaNumberDeleteVM
	{
		public VillaNumberDeleteVM()
		{
			VillaNumber = new VillaNumberDTO();
		}

		public VillaNumberDTO VillaNumber { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> VillaList { get; set; }
	}
}
