namespace MagicVilla_VillaAPI.Models.Dto
{
    public class LoginResponseDTO
    {
        public UserDto User { get; set; }

        // We dont below need as token contains a claim of role
        // public string Role { get; set; }
        public string Token { get; set; }
    }
}
