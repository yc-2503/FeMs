using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entitys
{
    public class User : IdentityUser<uint>
    {
        public User(string userName) : base(userName)
        {

        }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; private set; }
    }
}