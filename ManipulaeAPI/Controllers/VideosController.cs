using System;
using System.Threading.Tasks;
using Manipulae.Application.DTOs;
using Manipulae.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manipulae.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  
    public class VideosController : ControllerBase
    {
        private readonly IVideosService _videosService;
        private readonly IYouTubeService _youTubeService;

        public VideosController(IVideosService videosService, IYouTubeService youTubeService)
        {
            _videosService = videosService;
            _youTubeService = youTubeService;
        }

        // 1) Preencher o banco com dados da API do YouTube
        [HttpPost("fetch-youtube")]
        public async Task<IActionResult> FetchYouTubeData()
        {
            try
            {
                var videos = await _youTubeService.FetchYouTubeVideosAsync();
                var insertedVideos = new List<VideoDto>();
                foreach (var vid in videos)
                {
                    var inserted = await _videosService.InsertAsync(vid);
                    insertedVideos.Add(inserted);
                }
                return Ok(insertedVideos);
            }
            catch (Exception ex)
            {
                return StatusCode(500,"Houve um Erro ao Realizar a Busca");
            }
        }

        // 2.1) Filtrar
        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] string? title,
            [FromQuery] string? duration,
            [FromQuery] string? author,
            [FromQuery] DateTime? createdAfter,
            [FromQuery] string? q)
        {
            var result = await _videosService.FilterAsync(title, duration, author, createdAfter, q);
            return Ok(result);
        }

        // 2.2) Inserir
        [HttpPost("insert")]
        public async Task<IActionResult> Insert([FromBody] VideoDto videoDto)
        {
            if (videoDto == null) return BadRequest("VideoDto inválido.");

            var inserted = await _videosService.InsertAsync(videoDto);
            return Ok(inserted);
        }

        // 2.3) Atualizar
        [HttpPut("update")]
        public async Task<IActionResult> Update(int id,[FromBody] VideoDto videoDto)
        {
            if (videoDto == null || id  <= 0) return BadRequest("VideoDto inválido.");

            var updated = await _videosService.UpdateAsync(id, videoDto);
            if (updated == null) return NotFound("Vídeo não encontrado.");

            return Ok(updated);
        }

        // 2.4) Excluir (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var success = await _videosService.SoftDeleteAsync(id);
            if (!success) return NotFound("Vídeo não encontrado.");

            return Ok("Registro marcado como excluído (soft delete).");
        }
    }
}