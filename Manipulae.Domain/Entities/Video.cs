namespace Manipulae.Domain.Entities
{
    public class Video
    {
        public int Id { get; set; }

        // Dados básicos do vídeo
        public string Title { get; set; }
        public string Description { get; set; }
        public string Channel { get; set; }
        public string Duration { get; set; } // Ex.: "PT5M" 

        // Data de publicação
        public DateTime PublishDate { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
    }
}