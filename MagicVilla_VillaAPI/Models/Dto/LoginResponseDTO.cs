namespace MagicVilla_VillaAPI.Models.Dto
{
    public class LoginResponseDTO
    {
        public UserDto User { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
