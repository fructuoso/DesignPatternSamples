using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DesignPatternSamples.Application.DTO;
using DesignPatternSamples.Application.Services;
using DesignPatternSamples.WebAPI.Models;
using DesignPatternSamples.WebAPI.Models.Detran;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DesignPatternSamples.WebAPI.Controllers.Detran
{
    [Route("api/Detran/[controller]")]
    [ApiController]
    public class PontosController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IDetranVerificadorPontosCarteiraService _detranVerificadorPontosCarteiraService;
        
        public PontosController (IMapper mapper,
            IDetranVerificadorPontosCarteiraService detranVerificadorPontosCarteiraService)
        {
            _mapper = mapper;
            _detranVerificadorPontosCarteiraService = detranVerificadorPontosCarteiraService;
        }
        [HttpGet()]
        [ProducesResponseType(typeof(SuccessResultModel<IEnumerable<PontosCarteiraModel>>), StatusCodes.Status200OK)]
        public async  Task<ActionResult> Get([FromQuery]CondutorModel model)
        {
            var pontos = await _detranVerificadorPontosCarteiraService.ConsultarPontos(
                _mapper.Map<Condutor>(model));
            var resultado =
                new SuccessResultModel<IEnumerable<PontosCarteiraModel>>(
                    _mapper.Map<IEnumerable<PontosCarteiraModel>>(pontos));
            return Ok();
        }
    }
}
