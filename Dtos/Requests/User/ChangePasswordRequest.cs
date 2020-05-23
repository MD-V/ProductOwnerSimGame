namespace ProductOwnerSimGame.Dtos.Requests.User
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string PasswordRepeat { get; set; }
    }
}
