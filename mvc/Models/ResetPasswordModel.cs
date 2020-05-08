namespace mvc.Models
{
    public class ResetPasswordModel
    {
        public ResetPasswordModel()
        {            
        }

        public ResetPasswordModel(string token, string email)
        {
            Token = token;
            Email = email;
        }

        public string Token { get; }
        public string Email { get; }
    }
}