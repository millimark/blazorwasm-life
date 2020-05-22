using System.Collections.Generic;
using BlazorWasmLife.Shared;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConwayWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="ConwayLifeController" />.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConwayLifeController : ControllerBase
    {

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<ConwayLifeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConwayLifeController"/> class.
        /// </summary>
        /// <param name="logger">The logger<see cref="ILogger{ConwayLifeController}"/>.</param>
        public ConwayLifeController(ILogger<ConwayLifeController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Get a life matrix based on the named pattern.  
        /// </summary>
        /// <param name="pattern">name of the pattern, e.g., "glider" </param>
        /// <returns>an initialized matrix</returns>
        [HttpGet("{pattern}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EnableCors]
        public IActionResult Get(string pattern)
        {
            var rowList = LifePatterns.GetPattern(pattern);
            if (rowList == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    var matrix = LifeBoardInt.FromPattern(rowList);
                    return Ok(matrix);
                }
                catch
                {
                    return BadRequest();
                }
            }
        }


        /// <summary>
        /// takes a matrix of cells, applies rules and returns next generation
        /// </summary>
        /// <param name="cells">current generation matrix</param>
        /// <returns>the next generation of the matrix after Life rules are applied </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EnableCors]
        public IActionResult Post([FromBody] LifeBoardInt cells)
        {
            try
            {
                var matrix = (LifeBoardInt)cells.NextGeneration(cells);
                return Ok(matrix);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
