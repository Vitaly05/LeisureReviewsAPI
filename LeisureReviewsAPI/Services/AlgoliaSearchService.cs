using AutoMapper;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Services.Interfaces;
using Algolia.Search.Clients;
using LeisureReviewsAPI.Models.Search;
using Algolia.Search.Exceptions;

namespace LeisureReviewsAPI.Services
{
    public class AlgoliaSearchService : ISearchService
    {
        private readonly ISearchIndex reviewSearchIndex;

        private readonly ISearchIndex leisureSearchIndex;

        public AlgoliaSearchService(ISearchClient searchClient)
        {
            this.reviewSearchIndex = searchClient.InitIndex("reviews");
            this.leisureSearchIndex = searchClient.InitIndex("leisures");
        }

        public async Task CreateReviewAsync(Review review)
        {
            var reviewSearchModel = getSearchModel(review);
            try
            {
                if (reviewSearchModel.Content.Length > 9000)
                    reviewSearchModel.Content = "";
                await reviewSearchIndex.SaveObjectAsync(reviewSearchIndex);
            }
            catch (AlgoliaApiException ex)
            {
                reviewSearchModel.Comments.Clear();
                await reviewSearchIndex.SaveObjectAsync(reviewSearchIndex);
            }
        }


        public async Task UpdateReviewAsync(Review review)
        {
            var reviewSearchModel = getSearchModel(review);
            try
            {
                if (reviewSearchModel.Content.Length > 9000)
                    reviewSearchModel.Content = "";
                await reviewSearchIndex.PartialUpdateObjectAsync(reviewSearchModel);
            }
            catch (AlgoliaApiException ex)
            {
                Console.WriteLine(ex.Message);
                reviewSearchModel.Comments.Clear();
                await reviewSearchIndex.PartialUpdateObjectAsync(reviewSearchModel);
            }
            
        }

        public async Task DeleteReviewAsync(Review review) =>
            await reviewSearchIndex.DeleteObjectAsync(review.Id);

        public async Task CreateLeisureAsync(Leisure leisure) =>
            await leisureSearchIndex.SaveObjectAsync(new LeisureSearchModel { Name = leisure.Name });

        private ReviewSearchModel getSearchModel(Review review) =>
            new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewSearchModel>()
                .ForMember(r => r.Comments, opt => opt.MapFrom(r => getSearchModels<Comment, CommentSearchModel>(r.Comments)))
                .ForMember(r => r.Tags, opt => opt.MapFrom(r => getSearchModels<Tag, TagSearchModel>(r.Tags)))
                .ForMember(r => r.Leisure, opt => opt.MapFrom(r => r.Leisure.Name))
            )).Map<ReviewSearchModel>(review);

        private IEnumerable<TDestination> getSearchModels<TSourse, TDestination>(IEnumerable<TSourse> sourses)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<TSourse, TDestination>()));
            foreach (var sourse in sourses)
                yield return mapper.Map<TDestination>(sourse);
        }
    }
}
