using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// API Controller di prova
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILoggerService _logger;

        public HomeController(ILoggerService logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Endpoint GET per tutti i valori
        /// </summary>
        /// <returns></returns>
        // GET: api/Home
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Accesso a Home Controller - Get()");
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Endpoint GET per un singolo valore
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Home/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            _logger.LogDebug($"Valore passato: {id}");
            return "value";
        }

        // POST: api/Home
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _logger.LogError("Prova log errore");
        }

        // PUT: api/Home/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.LogWarn("Esempio di Warning");
        }
    }
}
