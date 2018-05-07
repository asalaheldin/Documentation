using Documentation.Data.DAL.Implementation;
using Documentation.Data.DAL.Intefraces;
using Documentation.Data.Entities;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.DependencyResolution
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            Scan(scan =>
            {
                scan.WithDefaultConventions();
                scan.TheCallingAssembly();
            });
            For<IRepository<User>>().Use<Repository<User>>();
            For<IRepository<Data.Entities.Type>>().Use<Repository<Data.Entities.Type>>();
            For<IRepository<Document>>().Use<Repository<Document>>();
        }
    }
}
