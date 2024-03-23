using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Villa_API.Models;
using Villa_API.Models.Dto.VillaNumber;
using Villa_API.Repository.IRepository;

namespace Villa_API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;

        public VillaNumberAPIController(IMapper mapper, IVillaNumberRepository villaNumberRepository, IVillaRepository dbVilla)
        {
            _mapper = mapper;
            _dbVillaNumber = villaNumberRepository;
            _response = new();
            _dbVilla = dbVilla;
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }



        [HttpGet("{id}", Name = "GetVillaNumber")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNmuberById(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("ID must be greater than 0");
                    return BadRequest(_response);
                }

                var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo == id);
                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Villa Number with given 'VillaNo' does not exist!");
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }



        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO villaNumberCreateDTO)
        {
            try
            {
                if (await _dbVillaNumber.GetAsync(x => x.VillaNo == villaNumberCreateDTO.VillaNo) != null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Villa Number with given 'VillaNo' already exists. Try different VillaNo!");
                    return BadRequest(_response);
                }

                if (await _dbVilla.GetAsync(x => x.Id == villaNumberCreateDTO.VillaId) == null)
                {
                    //ModelState.AddModelError("ErrorMessages", "Villa Id does not exist! Try valid one. ");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Villa ID does not exist. Try valid one!");
                    return BadRequest(_response);
                }

                if (villaNumberCreateDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Something went wrong. Try again later!");
                    return BadRequest(_response);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDTO);

                await _dbVillaNumber.CreateAsync(villaNumber);
                await _dbVillaNumber.SaveAsync();

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }



        [HttpDelete("{id}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("ID must be greater than 0.");
                    return BadRequest(_response);
                }

                var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo == id);

                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Villa Number with given 'VillaNo' does not exist.");
                    return NotFound(_response);
                }

                await _dbVillaNumber.RemoveAsync(villaNumber);
                await _dbVillaNumber.SaveAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }



        [HttpPut("{id}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO villaNumberUpdateDTO)
        {
            try
            {
                if (villaNumberUpdateDTO == null || id != villaNumberUpdateDTO.VillaNo)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Villa Number with given 'VillaNo' does not exist.");
                    return BadRequest(_response);
                }

                if (await _dbVilla.GetAsync(x => x.Id == villaNumberUpdateDTO.VillaId) == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Villa with given ID does not exist.");
                    return BadRequest(_response);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberUpdateDTO);

                await _dbVillaNumber.UpdateAsync(villaNumber);
                await _dbVillaNumber.SaveAsync();

                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }

            return _response;
        }
    }
}
