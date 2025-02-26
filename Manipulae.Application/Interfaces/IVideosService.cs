using Manipulae.Application.DTOs;

namespace Manipulae.Application.Interfaces
{
    public interface IVideosService
    {
        Task<IEnumerable<VideoDto>> FilterAsync(string? title, string? duration, string? author,
            DateTime? createdAfter, string? q);

        Task<VideoDto> InsertAsync(VideoDto video);
        Task<VideoDto> UpdateAsync(int id, VideoDto video);
        Task<bool> SoftDeleteAsync(int id);
    }
}