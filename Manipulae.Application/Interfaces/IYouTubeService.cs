using Manipulae.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manipulae.Application.Interfaces
{
    public interface IYouTubeService
    {
        Task<IEnumerable<VideoDto>> FetchYouTubeVideosAsync();
    }
}