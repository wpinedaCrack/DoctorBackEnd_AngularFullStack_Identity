﻿using BLL.Servicios.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using System.Net;

namespace API.Controllers
{
    //[Authorize(Roles="admin,agendador")]
    [Authorize(Policy = "adminagendadorrol")]   //Agregar politica
    public class EspecialidadController : BaseApiController
    {
        private readonly IEspecialidadServicio _especialidadServicio;
        ApiResponse _response;

        public EspecialidadController(IEspecialidadServicio especialidadServicio)
        {
            _especialidadServicio = especialidadServicio;
            _response = new();
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _response.Resultado = await _especialidadServicio.ObtenerTodos();
                _response.IsExitoso = true;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.Mensaje = ex.Message;
                _response.IsExitoso = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return Ok(_response);
        }

        [HttpGet("ListadoActivos")]
        public async Task<IActionResult> GetActivos()
        {
            try
            {
                _response.Resultado = await _especialidadServicio.ObtenerActivos();
                _response.IsExitoso = true;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.Mensaje = ex.Message;
                _response.IsExitoso = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return Ok(_response);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(EspecialidadDto modeloDto)
        {
            try
            {
                await _especialidadServicio.Agregar(modeloDto);
                _response.IsExitoso = true;
                _response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                _response.Mensaje = ex.Message;
                _response.IsExitoso = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return Ok(_response);
        }

        [HttpPut]
        public async Task<IActionResult> Editar(EspecialidadDto modeloDto)
        {
            try
            {
                await _especialidadServicio.Actualizar(modeloDto);
                _response.IsExitoso = true;
                _response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.Mensaje = ex.Message;
                _response.IsExitoso = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return Ok(_response);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _especialidadServicio.Remover(id);
                _response.IsExitoso = true;
                _response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                _response.Mensaje = ex.Message;
                _response.IsExitoso = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return Ok(_response);
        }
    }
}