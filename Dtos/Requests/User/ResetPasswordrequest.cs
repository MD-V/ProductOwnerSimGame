namespace ProductOwnerSimGame.Dtos.Requests.User
{
    public class ResetPasswordrequest
    {
        public string UserNameOrEmail { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }
}