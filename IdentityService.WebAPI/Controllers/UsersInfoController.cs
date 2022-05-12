using FeMs.Share;
using IdentityService.Domain;
using IdentityService.Domain.Entitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.WebAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    //[Authorize (Roles ="admin")]
    public class UsersInfoController : ControllerBase
    {
        private readonly IAuRepository auRepository;

        public UsersInfoController(IAuRepository auRepository)
        {
            this.auRepository = auRepository;
        }

        [HttpGet]
        public async Task<ActionResult<int>> GetUsersCount() => 
            await auRepository.GetUsersCount();
        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetUsers(int from,int to)
        {
            List<UserDTO> userList = new List<UserDTO>();
            if (to >= from)
            {
                List<User> users = await auRepository.GetUsers(from, to);
                foreach (var user in users)
                {
                    UserDTO userDTO = new UserDTO();
                    userDTO.UserName = user.UserName;
                    userDTO.Id = user.Id.ToString();
                    userDTO.PhoneNumber = user.PhoneNumber;
                    userDTO.Email = user.Email;
                    userList.Add(userDTO);
                }
            }
            return userList;

        }
    }
}
