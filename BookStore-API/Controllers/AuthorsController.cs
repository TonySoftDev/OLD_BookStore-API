using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint per interagire con gli Autori presenti nel database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, ILoggerService logger, IMapper mapper)
        {
            this._authorRepository = authorRepository;
            this._logger = logger;
            this._mapper = mapper;
        }

        /// <summary>
        /// Legge dal database l'elenco degli Autori
        /// </summary>
        /// <returns>Elenco degli Autori</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Chiamata GetAuthors()");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Chiamata GetAuthors() effettuata con successo.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Legge dal database un Autore dato il suo specifico ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Ritorna l'Autore richiesto</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Chiamata GetAuthor() con id: {id}");
                var author = await _authorRepository.FindById(id);
                if(author == null)
                {
                    _logger.LogWarn($"Autore con id:{id} non trovato.");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Chiamata GetAuthor() con id: {id} effettuata con successo.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Crea un nuovo Autore salvandolo sul database
        /// </summary>
        /// <param name="authorData"></param>
        /// <returns>Ritorna dati nuovo Autore</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorData)
        {
            try
            {
                _logger.LogInfo($"Chiamata Create()");
                if (authorData == null)
                {
                    _logger.LogWarn($"La richiesta non contiene dati.");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"La richiesta contiene dati non validi.");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorData);
                var isSuccess = await _authorRepository.Create(author);
                if(!isSuccess)
                {
                    return InternalError($"Creazione Autore fallita.");
                }
                _logger.LogInfo("Autore creato con successo.");
                return Created("Create", new { author });
            }
            catch (Exception ex)
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Modifica un Autore presente sul database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorData"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorData)
        {
            try
            {
                _logger.LogInfo($"Aggiornamento Autore id:{id}");
                if (id < 1 || authorData == null || id != authorData.Id)
                {
                    _logger.LogInfo("Aggiornamento fallito per dati non coerenti");
                    return BadRequest();
                }
                var isExists = await _authorRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"Author with id:{id} was not found");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogInfo("Aggiornamento fallito per dati incompleti");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorData);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError("Aggiornamento fallito.");
                }
                _logger.LogWarn($"Aggiornamento dell'Autore con id:{id} eseguito.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Cancella dal database l'Autore con l'id specificato
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo($"Author with id: {id} Delete Attempted");
                if (id < 1)
                {
                    _logger.LogWarn($"Author Delete failed with bad data");
                    return BadRequest();
                }
                var isExists = await _authorRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"Author with id:{id} was not found");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError($"Author Delete Failed");
                }
                _logger.LogWarn($"Author with id: {id} successfully deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }


        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Si è verificato un errore inatteso. Prego, contattare l'Amministratore del sistema.");
        }

    }
}