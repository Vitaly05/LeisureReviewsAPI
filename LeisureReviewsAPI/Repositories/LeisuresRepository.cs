using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Repositories.Interfaces;
using LeisureReviewsAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviewsAPI.Repositories
{
    public class LeisuresRepository : BaseRepository, ILeisuresRepository
    {
        private readonly ISearchService searchService;

        public LeisuresRepository(ApplicationContext context, ISearchService searchService) : base(context)
        {
            this.searchService = searchService;
        }

        public async Task<Leisure> AddAsync(string name)
        {
            if (await context.Leisures.AnyAsync(l => l.Name == name)) 
                return await context.Leisures.FirstOrDefaultAsync(l => l.Name == name);
            var leisure = new Leisure { Name = name };
            await saveAsync(leisure);
            return leisure;
        }

        public async Task<Leisure> GetAsync(string id) =>
            await context.Leisures.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);

        public async Task<Leisure> GetFromReviewAsync(string reviewId) =>
            await context.Reviews.Where(r => r.Id == reviewId).Select(r => r.Leisure).FirstOrDefaultAsync();

        private async Task saveAsync(Leisure leisure)
        {
            await context.Leisures.AddAsync(leisure);
            await searchService.CreateLeisureAsync(leisure);
            await context.SaveChangesAsync();
        }
    }
}
