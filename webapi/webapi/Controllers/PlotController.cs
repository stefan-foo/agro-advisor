﻿using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    [Route("plot")]
    [ApiController]
    public class PlotController: ControllerBase
    {
        private readonly ILogger<PlotController> _logger;
        private readonly PlotService _plotService;
        public PlotController(
            ILogger<PlotController> logger,
            PlotService plotService)
        {
            _plotService = plotService;
            _logger = logger;
        }
        [HttpPost("add")]
        public async Task<ActionResult> AddPlot([FromBody] PlotDTO plot)
        {
            try
            {
                await _plotService.CreateAsync(plot);
                return Ok("New plot added successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //TODO:Koristi ID trenutnog korisnika umesto hardkodirani
        //[Authorize]
        [HttpGet("{plotId}")]
        public async Task<ActionResult> GetPlot(string plotId)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value;
                //userId= "63df951c945d31aa29069e31";
                if (userId == null)
                {
                    return Unauthorized("Greska pri autentifikaciji");
                }

                var result = await _plotService.GetPlotAsync(userId, plotId);
                if (result == null)
                    return BadRequest("Ne postoji plot za trazeni ID");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("plots")]
        public async Task<ActionResult> GetUserPlots()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value;
                if (userId == null)
                {
                    return Unauthorized("Greska pri autentifikaciji");
                }

                var result = await _plotService.GetUserPlotsAsync(userId);
                if (result == null)
                    return BadRequest("Korisnik nema registrovanog zemljista!");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
