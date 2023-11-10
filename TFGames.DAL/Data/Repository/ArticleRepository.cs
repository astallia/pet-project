using Microsoft.EntityFrameworkCore;
using TFGames.DAL.Entities;
using TFGames.Common.Extensions;
using TFGames.Common.Enums;
using TFGames.DAL.Search;

namespace TFGames.DAL.Data.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly DataContext _dataContext;

        public ArticleRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task Create(Article article)
        {
            _dataContext.Add(article);

            await SaveChanges();
        }

        public async Task SaveImage(ArticleImage image)
        {
            _dataContext.Add(image);

            await SaveChanges();
        }

        public async Task<Dictionary<Guid, ArticleImage>> GetImages(params Guid[] ids)
        {
            return await _dataContext.ArticleImages
                .Where(i => ids.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id, i => i);
        }

        public async Task Update(Article article)
        {
            _dataContext.Update(article);

            await SaveChanges();
        }

        public async ValueTask<Article> FindByIdWithInclude(Guid id)
        {
            return await _dataContext.Article
                .Include(x => x.GameInfo)
                .Include(x => x.Tags)
                .Include(x => x.Author)
                .Include(x => x.Author.Avatar)
                .Include(x => x.Content)
                .Include(x => x.Likes)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task PutLike(User user, Article article)
        {
            var hasAlreadyLiked = user.Favorites
                .FirstOrDefault(f => f.Id == article.Id);

            if (hasAlreadyLiked != null)
            {
                user.Favorites.Remove(hasAlreadyLiked);
            }
            else
            {
                user.Favorites.Add(article);
            }

            await SaveChanges();
        }

        public async ValueTask<List<User>> GetAllAuthors(Filters filters, OrderByProperties orderBy)
        {
            IQueryable<Article> query = _dataContext.Article
                .Include(auth => auth.Author);

            if (filters.Search != null)
            {
                query = query.Where(a => a.Author.Surname.Contains(filters.Search) ||
                    a.Author.Name.Contains(filters.Search) ||
                    a.Author.UserName.Contains(filters.Search));
            }

            if (orderBy != null)
            {
                switch (orderBy.Order)
                {
                    case OrderBy.CreatedDate:
                        query = orderBy.SortParameters == SortParameters.NewPosts ? query.OrderBy(a => a.Created) : query.OrderByDescending(a => a.Created);
                        break;
                }
            }

            var authors = await query
                .Include(a => a.Author.Avatar)
                .Select(a => a.Author)
                .Distinct()
                .ToListAsync();

            return authors;
        }

        public async ValueTask<(List<Article> Articles, int Count)> GetPage(Filters filters, OrderByProperties orderBy, int page, int size, Guid articleId, FindBy? find)
        {
            IQueryable<Article> query = _dataContext.Article
                .Include(x => x.Author)
                .Include(x => x.Likes);

            if (filters.User != null)
            {
                query = query.Where(a => a.Likes.Any(u => u == filters.User));
            }

            if (filters.Search != null && (!find.HasValue || find.Value == 0))
            {
                query = query.Where(a => a.Name.Contains(filters.Search) ||
                    a.Comments.Any(c => c.Article.Comments.Any(c => c.Content.Contains(filters.Search))) ||
                    a.Author.Name.Contains(filters.Search));
            }

            if (find.HasValue && filters.Search != null)
            {
                switch (find)
                {
                    case FindBy.UserArticles:
                        query = query.Where(a => a.Author.Id == filters.Search && a.Id != articleId);
                        break;
                    case FindBy.Username:
                        query = query.Where(a => a.Author.UserName == filters.Search);
                        break;
                    case FindBy.Tags:
                        query = query.Where(a => a.Tags.Any(t => t.Name == "#" + filters.Search || t.Name == filters.Search));
                        break;
                }
            }

            if (orderBy != null)
            {
                switch (orderBy.Order)
                {
                    case OrderBy.CreatedDate:
                        query = orderBy.SortParameters == SortParameters.NewPosts ? query.OrderBy(a => a.Created) : query.OrderByDescending(a => a.Created);
                        break;
                    case OrderBy.Likes:
                        query = query.OrderByDescending(a => a.Likes.Count);
                        break;
                }
            }

            int totalArticles = (int)Math.Ceiling(query.Count() / (double)size);

            return (await query
                .Page(page, size)
                .Include(x => x.GameInfo)
                .Include(x => x.Tags)
                .Include(x => x.Author.Avatar)
                .ToListAsync(), totalArticles);
        }

        public async ValueTask<List<User>> GetTopAuthors(int size)
        {
            var userLikes = await _dataContext.Article
                .Include(a => a.Author)
                .Include(a => a.Author.Avatar)
                .Include(a => a.Likes)
                .GroupBy(a => a.Author.Id)
                .Select(g => new
                {
                    AuthorId = g.Key,
                    Authors = g.Select(x => x.Author),
                    Likes = g.Select(x => x.Likes.Count)
                })
                .Select(x => new
                {
                    Author = x.Authors.First(),
                    TotalLikes = x.Likes.Sum()
                })
                .ToListAsync();

            return userLikes
                .OrderByDescending(x => x.TotalLikes)
                .Take(size)
                .Select(x => x.Author)
                .ToList();
        }

        public async Task SaveChanges()
        {
            await _dataContext.SaveChangesAsync();
        }

        public async Task Delete(Guid articleId)
        {
            var article = await _dataContext.Article
                .Include(c => c.Comments)
                .Include(t => t.Tags)
                .Include(m => m.MainImage)
                .Include(c => c.Content)
                .Include(g => g.GameInfo)
                .Include(x => x.Likes)
                .SingleAsync(a => a.Id == articleId);

            _dataContext.ArticleImages.Remove(article.MainImage);

            _dataContext.ArticleContents.Remove(article.Content);

            _dataContext.GameInfo.Remove(article.GameInfo);

            _dataContext.Remove(article);

            await SaveChanges();
        }
    }
}
