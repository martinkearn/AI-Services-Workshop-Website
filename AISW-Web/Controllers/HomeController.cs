using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AISW_Web.Models;
using AISW_Web.Interfaces;

namespace AISW_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public HomeController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var links = await _storeRepository.GetLinks();

            var linkCategories = links.Select(l => l.Category).Distinct();

            var linkGroups = new List<LinkGroup>();
            foreach (var linkCategory in linkCategories)
            {
                linkGroups.Add(new LinkGroup() { Category = linkCategory, Links = links.Where(l => l.Category == linkCategory).ToList() });
            }

            return View(linkGroups);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
