using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servexa.Application.Interfaces;

namespace Servexa.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : BaseController
    {
        private readonly IAdminCategoryService _service;

        public CategoriesController(IAdminCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Success(result, "Categories fetched successfully");
        }
    }
}
