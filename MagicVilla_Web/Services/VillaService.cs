using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class VillaService : BaseService, IVillaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;
        public VillaService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }
        public Task<T> CreateAsync<T>(VillaCreateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Token = token,
                Data = dto,
                Url = villaUrl + "/api/v1/villaAPI"
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Token = token,
                Url = villaUrl + "/api/v1/villaAPI/" + id
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Token = token,
                Url = villaUrl + "/api/v1/villaAPI"
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                Token = token,
                Url = villaUrl + "/api/v1/villaAPI/" + id
            });
        }

        public Task<T> UpdateAsync<T>(VillaUpdateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Token = token,
                Data = dto,
                Url = villaUrl + "/api/v1/villaAPI/" + dto.Id      // If you want, you can remove the id from the update request and use only the body
            });
        }
    }
}
