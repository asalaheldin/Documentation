namespace Documentation.Data.Migrations
{
    using Documentation.Data.DAL.Intefraces;
    using Documentation.Data.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Documentation.Data.DAL.Implementation.DocumentationContext>
    {
        private readonly Lazy<IRepository<User>> _userRepo;
        private readonly Lazy<IRepository<Data.Entities.Type>> _typeRepo;
        public Configuration(Lazy<IRepository<User>> userRepo, Lazy<IRepository<Data.Entities.Type>> typeRepo)
        {
            AutomaticMigrationsEnabled = false;
            _userRepo = userRepo;
            _typeRepo = typeRepo;
        }

        private IRepository<User> UserRepo => _userRepo.Value;
        private IRepository<Data.Entities.Type> TypeRepo => _typeRepo.Value;

        protected override void Seed(Documentation.Data.DAL.Implementation.DocumentationContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var user = UserRepo.FindBy(x => x.UserName == "AHassan").FirstOrDefault();
            if(user == null)
            {
                UserRepo.Insert(new User() {
                    UserName = "AHassan",
                    Password = "",
                    
                });
            }
        }
    }
}
