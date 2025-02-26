using System.Text.Json;
using Manipulae.Application.DTOs;
using Manipulae.Application.Interfaces;
using Manipulae.Domain.Entities;

namespace Manipulae.Infrastructure.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly string _apiKey; // Recebida via variável de ambiente
        private readonly HttpClient _httpClient;

        public YouTubeService()
        {
            _apiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY");
            if (string.IsNullOrEmpty(_apiKey))
                throw new InvalidOperationException("YouTube API key not found in environment variables.");

            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<VideoDto>> FetchYouTubeVideosAsync()
        {
            // Query: vídeos brasileiros, 2022, relacionados à manipulação de medicamentos.
            string searchUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=10" +
                               $"&q=manipulacao+de+medicamentos&regionCode=BR&publishedAfter=2022-01-01T00:00:00Z" +
                               $"&publishedBefore=2022-12-31T23:59:59Z&type=video&key={_apiKey}";

            var response = await _httpClient.GetAsync(searchUrl);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);

            var videos = new List<VideoDto>();
            var videoIds = new List<string>();

            // Processa a resposta do endpoint "search"
            if (doc.RootElement.TryGetProperty("items", out JsonElement items))
            {
                foreach (var item in items.EnumerateArray())
                {
                    // Extrai o ID do vídeo
                    string videoId = null;
                    if (item.TryGetProperty("id", out JsonElement idElement) &&
                        idElement.TryGetProperty("videoId", out JsonElement videoIdElement))
                    {
                        videoId = videoIdElement.GetString();
                        videoIds.Add(videoId);
                    }

                    var snippet = item.GetProperty("snippet");
                    var title = snippet.GetProperty("title").GetString();
                    var description = snippet.GetProperty("description").GetString();
                    var channel = snippet.GetProperty("channelTitle").GetString();
                    var publishDateString = snippet.GetProperty("publishedAt").GetString();
                    DateTime.TryParse(publishDateString, out DateTime publishDate);

                    videos.Add(new VideoDto
                    {
                        VideoId = videoId, // Armazena o ID do vídeo
                        Title = title,
                        Description = description,
                        Channel = channel,
                        Duration = "N/A", // Valor padrão; será atualizado em seguida
                        PublishDate = publishDate
                    });
                }
            }

            // Se obtivemos IDs de vídeos, faça uma chamada em lote para obter as durações
            if (videoIds.Any())
            {
                string ids = string.Join(",", videoIds);
                string detailsUrl = $"https://www.googleapis.com/youtube/v3/videos?part=contentDetails&id={ids}&key={_apiKey}";
                var detailsResponse = await _httpClient.GetAsync(detailsUrl);
                detailsResponse.EnsureSuccessStatusCode();

                var detailsJson = await detailsResponse.Content.ReadAsStringAsync();
                using var detailsDoc = JsonDocument.Parse(detailsJson);

                // Mapeia o videoId para a duração
                var durationDict = new Dictionary<string, string>();
                if (detailsDoc.RootElement.TryGetProperty("items", out JsonElement detailsItems))
                {
                    foreach (var detail in detailsItems.EnumerateArray())
                    {
                        var id = detail.GetProperty("id").GetString();
                        var duration = detail.GetProperty("contentDetails").GetProperty("duration").GetString();
                        durationDict[id] = duration;
                    }
                }

                // Atualiza a propriedade Duration nos objetos VideoDto
                foreach (var video in videos)
                {
                    if (!string.IsNullOrEmpty(video.VideoId) && durationDict.TryGetValue(video.VideoId, out string dur))
                    {
                        video.Duration = dur;
                    }
                }
            }

            return videos;
        }
    }
}