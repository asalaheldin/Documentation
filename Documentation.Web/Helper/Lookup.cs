using Documentation.Data.DAL.Intefraces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Documentation.Web.Helper
{
    public class Lookup
    {
        public static List<Data.Entities.Type> GetTypes()
        {
            var typeRepo = DocumentationContainerHelper.Container.GetInstance<IRepository<Data.Entities.Type>>();
            var list = typeRepo.GetAll();
            return list.OrderBy(x => x.Id).ToList();
        }
    }
}