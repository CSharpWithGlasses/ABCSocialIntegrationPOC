using ABC.Data;
using ABC.Shared.Interfaces;
using ABC.Shared.Interfaces.Repository;
using ABC.Shared.Models;
using ABC.Integration.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABC.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialMediaController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ISocialStatistics _statisticsManager;
        private readonly ISocialManager _twitterManager;
        public SocialMediaController(ISocialStatistics statisticsManager, TwitterManager twitterManager, ILogger<SocialMediaController> logger)
        {
            _twitterManager = twitterManager;
            _logger = logger;
            _statisticsManager = statisticsManager;
        }



        [HttpGet("StartProcessing")]
        public async Task<ActionResult> StartProcessing()
        {
            try
            {
                _logger.LogInformation("Preparing to Start Processing...");
                await Task.Run(() => _twitterManager.StartService());

                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                var ohNo = new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
                return ohNo;
            }

        }

        [HttpGet("Stats")]
        public async Task<ActionResult<SocialStatistics>> GetStatistics()
        {
            try
            {
                _logger.LogInformation("Preparing to Gather Statistics...");
                var result = await Task.Run(() => _statisticsManager.GetTwitterStatistics(DateTime.Now));
                return Ok(result);
            }
            catch (Exception ex) {
                _logger.LogError(ex.ToString());
                var ohNo = new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
                return ohNo;
            }
        }




    }
}
