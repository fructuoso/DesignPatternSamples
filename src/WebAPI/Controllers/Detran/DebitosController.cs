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
    public class DebitosController : ControllerBase
    {
        private readonly IMapper _Mapper;
        private readonly IDetranVerificadorDebitosService _DetranVerificadorDebitosServices;

        public DebitosController(IMapper mapper, IDetranVerificadorDebitosService detranVerificadorDebitosServices)
        {
            _Mapper = mapper;
            _DetranVerificadorDebitosServices = detranVerificadorDebitosServices;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(SuccessResultModel<IEnumerable<DebitoVeiculoModel>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery]VeiculoModel model)
        {
            var debitos = await _DetranVerificadorDebitosServices.ConsultarDebitos(_Mapper.Map<Veiculo>(model));

            var result = new SuccessResultModel<IEnumerable<DebitoVeiculoModel>>(_Mapper.Map<IEnumerable<DebitoVeiculoModel>>(debitos));

            return Ok(result);
        }
    }
}