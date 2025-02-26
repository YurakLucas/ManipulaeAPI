using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manipulae.Application.DTOs;
using Manipulae.Application.Interfaces;
using Manipulae.Domain.Entities;
using Manipulae.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Manipulae.Infrastructure.Services
{
    public class VideosService : IVideosService
    {
        private readonly ApplicationDbContext _dbContext;

        public VideosService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<VideoDto>> FilterAsync(
            string? title, string? duration, string? author, DateTime? createdAfter, string? q)
        {
            var query = _dbContext.Videos.AsQueryable();

            // Somente vídeos não deletados
            query = query.Where(v => !v.IsDeleted);

            if (!string.IsNullOrEmpty(title))
                query = query.Where(v => v.Title.Contains(title));

            if (!string.IsNullOrEmpty(duration))
                query = query.Where(v => v.Duration == duration);

            if (!string.IsNullOrEmpty(author))
                query = query.Where(v => v.Channel.Contains(author));

            if (createdAfter.HasValue)
                query = query.Where(v => v.PublishDate > createdAfter.Value);

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(v =>
                    v.Title.Contains(q) ||
                    v.Description.Contains(q) ||
                    v.Channel.Contains(q));
            }

            var result = await query.ToListAsync();
            return result.Select(MapToDto);
        }

        public async Task<VideoDto> InsertAsync(VideoDto videoDto)
        {
            videoDto.Id = 0;

            var entity = MapToEntity(videoDto);
            _dbContext.Videos.Add(entity);
            await _dbContext.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<VideoDto> UpdateAsync(int id, VideoDto videoDto)
        {
            var entity = await _dbContext.Videos.FindAsync(id);
            if (entity == null) return null;

            entity.Title = videoDto.Title;
            entity.Description = videoDto.Description;
            entity.Channel = videoDto.Channel;
            entity.Duration = videoDto.Duration;
            entity.PublishDate = videoDto.PublishDate;
            // IsDeleted não é atualizado aqui, pois é controlado pelo soft delete

            _dbContext.Videos.Update(entity);
            await _dbContext.SaveChangesAsync();

            return MapToDto(entity);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _dbContext.Videos.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private VideoDto MapToDto(Video v) => new()
        {
            Id = v.Id,
            Title = v.Title,
            Description = v.Description,
            Channel = v.Channel,
            Duration = v.Duration,
            PublishDate = v.PublishDate,
            IsDeleted = v.IsDeleted
        };

        private Video MapToEntity(VideoDto dto) => new()
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            Channel = dto.Channel,
            Duration = dto.Duration,
            PublishDate = dto.PublishDate,
            IsDeleted = dto.IsDeleted
        };
    }
}