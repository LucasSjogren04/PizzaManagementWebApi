namespace Tomaso_Pizza.Data.DTO
{
    public class UpdateUserDTO
    {
        public class ChangePasswordModel
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class UpdateProfileModel
        {
            public string NewEmail { get; set; }
            public string NewUsername { get; set; }
            public string NewPhoneNumber { get; set; }
        }
    }
}
