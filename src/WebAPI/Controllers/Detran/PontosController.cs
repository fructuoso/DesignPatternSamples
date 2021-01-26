using AutoMapper;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using DesignPatternSamples.WebAPI.Models;
using DesignPatternSamples.WebAPI.Models.Detran;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesignPatternSamples.WebAPI.Controllers.Detran
{
    [Route("api/Detran/[controller]")]
    [ApiController]
    public class PontosController : ControllerBase
    {
        private readonly IMapper _Mapper;
        private readonly IDetranPontosCarteiraService _DetranPontosCarteiraServices;

        public PontosController(IMapper mapper, IDetranPontosCarteiraServices detranPontosCarteiraServices)
        {
            _Mapper = mapper;
            _DetranPontosCarteiraServices = detranPontosCarteiraServices;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(SuccessResultModel<IEnumerable<PontosCarteiraModel>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery]CarteiraModel model)
        {
            var pontos = await _DetranPontosCarteiraServices.ConsultarPontos(_Mapper.Map<Carteira>(model));

            var result = new SuccessResultModel<IEnumerable<PontosCarteiraModel>>(_Mapper.Map<IEnumerable<PontosCarteiraModel>>(pontos));

            return Ok(result);
        }
    }
}