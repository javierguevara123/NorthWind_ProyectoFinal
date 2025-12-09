namespace NorthWind.Membership.Entities.UserLogin
{
    public class TokensDto(string accessToken)
    {
        public string AccessToken => accessToken;
    }

}
