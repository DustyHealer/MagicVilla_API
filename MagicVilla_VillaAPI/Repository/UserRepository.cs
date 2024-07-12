using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager; // MS Identity user manager for managing user data
        private readonly RoleManager<IdentityRole> _roleManager; // MS Identity user manager for managing user roles
        private string secretKey;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string username)
        {
            // Older code with local user class
            //var user = _db.LocalUsers.FirstOrDefault(u => u.UserName == username);
            //if (user == null) 
            //{
            //    return true;
            //}
            //return false;

            // Code which makes use of ms identity features
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            // Ignose case matching for username
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user == null || !isValid)
            {
                return new LoginResponseDTO()
                {
                    Token = string.Empty,
                    User = null
                };
            }

            // Retrieve roles from the ms identity
            var roles = await _userManager.GetRolesAsync(user);

            // If user was found, we have to generate the JWT Token
            var tokenHandler = new JwtSecurityTokenHandler(); // Token handler to create a jwt token
            var key = Encoding.ASCII.GetBytes(secretKey); // Convert key from string to bytes

            // Declare token descriptor which contains claims about username, roles, token expiration etc
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDto>(user),
                Role = roles.FirstOrDefault(),
            };
            return loginResponseDTO;
        }

        public async Task<UserDto> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.UserName,
                Email = registrationRequestDTO.UserName,
                NormalizedEmail = registrationRequestDTO.UserName.ToUpper(),
                Name = registrationRequestDTO.Name,
            };

            try
            {
                // Create a new user using user manager. Pass the string password, it will automatically hash it and store in db
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }
                    await _userManager.AddToRoleAsync(user, "admin");
                    var userToReturn = _db.ApplicationUsers.FirstOrDefault(U => U.UserName == registrationRequestDTO.UserName);
                    //return new UserDto()
                    //{
                    //    Id = userToReturn.Id,
                    //    UserName = userToReturn.UserName,
                    //    Name = userToReturn.Name,
                    //};
                    return _mapper.Map<UserDto>(userToReturn);
                }
            }
            catch (Exception ex)
            {

            }

            return new UserDto();
        }
    }
}
