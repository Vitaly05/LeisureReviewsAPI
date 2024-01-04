using LeisureReviewsAPI.Models.Database;

namespace LeisureReviewsAPI.Models.Dto
{
    public class AccountInfoDto
    {
        public AccountInfoDto() { }
        public AccountInfoDto(AccountInfo accountInfo)
        {
            this.IsAuthorized = accountInfo.IsAuthorized;
            if (accountInfo.CurrentUser is not null)
                this.CurrentUser = new UserDto(accountInfo.CurrentUser);
        }

        public bool IsAuthorized { get; set; } = false;

        public UserDto CurrentUser { get; set; }
    }
}
