using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISW_Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AISW_Web.Controllers
{
    public class RedirectController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public RedirectController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<IActionResult> Index(string label)
        {
            var link = await _storeRepository.GetLink(label);

            if (link != null)
            {
                return Redirect(link.Url);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}