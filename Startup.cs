using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using MovieShop.Models;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieShop.Startup))]
namespace MovieShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoleandUsers();
        }

        private void CreateRoleandUsers()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var rolemanager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var usermanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            if(!rolemanager.RoleExists("Admin"))
            {
                //First we create admin role
                var role = new IdentityRole();
                role.Name = "Admin";
                rolemanager.Create(role);
            }
            if(usermanager.FindByEmail("simon@movieshop.com")==null)
            {
                //Create SuperUser/Admin for site
                var user = new ApplicationUser();
                user.UserName = "Simon";
                user.Email = "simon@movieshop.com";
                string pswrd = "Sb@123";
                var chkres = usermanager.Create(user, pswrd);

                //On Successassign assign admin role to above user
                if(chkres.Succeeded)
                {
                    usermanager.AddToRole(user.Id, "Admin");
                }

            }
            //Adding Manager Role
            if (!rolemanager.RoleExists("Manager"))
            {
                //First we create admin role
                var role = new IdentityRole();
                role.Name = "Manager";
                rolemanager.Create(role);
            }
        }
    }
}
