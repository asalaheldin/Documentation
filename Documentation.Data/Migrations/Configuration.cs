namespace Documentation.Data.Migrations
{
    using Documentation.Data.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Documentation.Data.DAL.Implementation.DocumentationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Documentation.Data.DAL.Implementation.DocumentationContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Users.AddOrUpdate(p => p.Id,
                new User()
                {
                    UserName = "ahassan@gmail.com",
                    Password = @"�
�9I�Y��V�W�� > ",
                    FirstName = "Amr",
                    LastName = "Salah Eldin"

                });

            context.Types.AddOrUpdate(p => p.Id,
                new Entities.Type()
                {
                    Id = 1,
                    Name = "Incoming"
                },
                new Entities.Type()
                {
                    Id = 1,
                    Name = "Outgoing"
                });
        }
    }
}
