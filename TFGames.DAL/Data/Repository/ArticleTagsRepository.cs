using Microsoft.EntityFrameworkCore;
using TFGames.DAL.Entities;
using TFGames.Common.Enums;
using TFGames.DAL.Search;

namespace TFGames.DAL.Data.Repository
{
    public class ArticleTagsRepository : IArticleTagsRepository
    {
        private readonly DataContext _context;

        public ArticleTagsRepository(DataContext context)
        {
            _context = context;
        }

        public async ValueTask<List<Tag>> GetTop(int size)
        {
            var tags = await _context.Tags
                .Include(t => t.Articles)
                .Where(t => t.Articles.Any())
                .OrderByDescending(t => t.Articles.Count)
                .Take(size)
                .ToListAsync();
                
            return tags;
        }

        public async ValueTask<List<Tag>> FindByNames(List<string> names)
        {
            return await _context.Tags
                .Where(t => names.Contains(t.Name))
                .ToListAsync();
        }

        public async ValueTask<List<Tag>> GetAll(Filters filters, OrderByProperties orderBy)
        {
            IQueryable<Tag> tags = _context.Tags
                .Include(a => a.Articles);

            if (filters.Search != null)
            {
                tags = tags.Where(c => c.Name.Contains(filters.Search));
            }

            if (orderBy != null)

            {
                switch (orderBy.Order)
                {
                    case OrderBy.CreatedDate:
                        tags = orderBy.SortParameters == SortParameters.NewPosts ? tags.OrderBy(a => a.Articles.FirstOrDefault().Created) : tags.OrderByDescending(a => a.Articles.FirstOrDefault().Created);
                        break;
                }
            }

            return await tags
                .ToListAsync();
        }
    }
}