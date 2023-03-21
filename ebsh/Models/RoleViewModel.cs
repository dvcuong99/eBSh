using Identity.Core;

namespace eBSH.Models
{
    public class RoleViewModel
    {
        public RoleViewModel() { }
        public RoleViewModel(IdentityRole role)
        {
            RoleID = role.RoleId.ToString();
            Name = role.Name;
        }
        public string RoleID { get; set; }
        public string Name { get; set; }
    }
}