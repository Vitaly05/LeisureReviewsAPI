using LeisureReviewsAPI.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models.Dto
{
    public class LeisureDto
    {
        public LeisureDto() { }
        public LeisureDto(Leisure leisure, double averageRate)
        {
            this.Id = leisure.Id;
            this.Name = leisure.Name;
            this.AverageRate = averageRate;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public double AverageRate { get; set; }
    }
}
