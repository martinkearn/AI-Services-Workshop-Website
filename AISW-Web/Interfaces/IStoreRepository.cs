using AISW_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISW_Web.Interfaces
{
    public interface IStoreRepository
    {
        Task StoreLink(Link Link);

        Task DeleteLink(string Id);

        Task<Link> GetLink(string Id);

        Task<List<Link>> GetLinks();
    }
}
