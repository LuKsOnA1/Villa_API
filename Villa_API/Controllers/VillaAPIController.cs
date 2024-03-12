using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Villa_API.Data;
using Villa_API.Models.Dto;

namespace Villa_API.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }

        [HttpGet("id", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVillaById(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            if (VillaStore.villaList.FirstOrDefault(x => x.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("", "Villa already exists");
                return BadRequest(ModelState);
            }

            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDTO.Id = VillaStore.villaList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("id", Name = "DeleteVilla")]
        public IActionResult Delete(int id)
        {
            // Checking if id exists
            if(id == 0)
            {
                return BadRequest();
            }

            // Finding villa by id
            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);

            if(villa == null)
            {
                return NotFound();
            }

            // Removing villa which we already found
            VillaStore.villaList.Remove(villa);

            return NoContent();
        }
    }
}
