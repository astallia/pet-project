using Microsoft.EntityFrameworkCore;
using TFGames.DAL.Entities;
using TFGames.Common.Enums;
using TFGames.DAL.Search;

namespace TFGames.DAL.Data.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<Comment> _model;

        public CommentRepository(DataContext context)
        {
            _context = context;
            _model = context.Comments;
        }

        public async Task Create(Comment comment)
        {
            await _model.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Comment comment)
        {
            comment.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async ValueTask<List<Comment>> GetAll(Filters filters, OrderByProperties orderBy)
        {
            IQueryable<Comment> comments = _context.Comments
                .Include(a => a.Author)
                .Include(x => x.Article);

            if (filters.Search != null)
            {
                comments = comments.Where(c => c.Content.Contains(filters.Search) && !c.IsDeleted);
            }

            if (orderBy != null)
            {
                switch (orderBy.Order)
                {
                    case OrderBy.CreatedDate:
                        comments = orderBy.SortParameters == SortParameters.NewPosts ? comments.OrderBy(a => a.Article.Created) : comments.OrderByDescending(a => a.Article.Created);
                        break;
                }
            }

            return await comments
                .Include(c => c.Author.Avatar)
                .ToListAsync();
        }

        public async Task<List<Comment>> FindAllByArticleId(Guid articleId)
        {
            var comments = await _model
                            .Where(c => c.Article.Id == articleId && c.Parent == null && !c.IsDeleted)
                            .Include(c => c.Article)
                            .OrderByDescending(c => c.CreatedAt)
                            .ToListAsync();

            foreach (var comment in comments)
            {
                await FindRepliesByComment(comment);
            }

            return comments;
        }

        private async Task FindRepliesByComment(Comment comment)
        {
            await _context.Entry(comment)
                .Reference(c => c.Author)
                .LoadAsync();
            await _context.Entry(comment.Author)
                .Reference(a => a.Avatar)
                .LoadAsync();
            await _context.Entry(comment)
                .Reference(c => c.Article)
                .LoadAsync();
            await _context.Entry(comment)
                .Collection(c => c.Replies)
                .LoadAsync();

            foreach (var reply in comment.Replies)
            {
                await FindRepliesByComment(reply);
            }
        }

        public async Task<Comment> FindById(Guid id)
        {
            var comment = await _model
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return null;
            }

            await FindRepliesByComment(comment);

            return comment;
        }

        public async Task Update(Comment comment)
        {
            _model.Update(comment);
            await _context.SaveChangesAsync();
        }
    }
}