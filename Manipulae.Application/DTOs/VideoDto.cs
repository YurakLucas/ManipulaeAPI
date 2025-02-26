using System.Text.Json.Serialization;

namespace Manipulae.Application.DTOs
{
    public class VideoDto
    {
        public int Id { get; set; }

        [JsonIgnore] 
        public string? VideoId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Channel { get; set; }

        public string Duration { get; set; }

        public DateTime PublishDate { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}