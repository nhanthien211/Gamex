using System.Web.Http;
using GamexApiService.Interface;
using GamexApiService.ViewModel;
using GamexEntity.Constant;

namespace GamexApi.Controllers {
    [Authorize(Roles = AccountRole.User)]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class CompanyController : ApiController {
        private ICompanyService _companyService;

        public CompanyController(ICompanyService companyService) {
            _companyService = companyService;
        }

        [HttpGet]
        [Route("company")]
        public CompanyViewModel GetCompany(string id) {
            return _companyService.GetCompany(id);
        }
    }
}
