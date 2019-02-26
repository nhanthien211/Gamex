using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GamexApi.Models;
using GamexEntity;
using GamexRepository;

namespace GamexApi.Controllers {
    [Authorize]
    [System.Web.Mvc.RequireHttps]
    [RoutePrefix("api")]
    public class ExhibitionController : ApiController {

        private IRepository<Exhibition> _exhibitionRepo;

        public ExhibitionController() {
            _exhibitionRepo = new Repository<Exhibition>(new GamexContext());
        }

        // GET /exhibition
        [Route("exhibition")]
        public List<ExhibitionViewModel> GetExhibitions() {

            var exhibitions = _exhibitionRepo.GetAll().ToList();

            var exhibitionViewModels = new List<ExhibitionViewModel>();
            exhibitions.ForEach(exhibition => {
                exhibitionViewModels.Add(new ExhibitionViewModel() {
                    ExhibitionId = exhibition.ExhibitionId,
                    Name = exhibition.Name,
                    Address = exhibition.Address,
                    Description = exhibition.Description,
                    Qr = exhibition.QR,
                    StartDate = exhibition.StartDate.ToString(CultureInfo.InvariantCulture),
                    EndDate = exhibition.EndDate.ToString(CultureInfo.InvariantCulture),
                    Location = exhibition.Location == null ? string.Empty : exhibition.Location.ToString(),
                    Logo = exhibition.Logo,
                    OrganizerId = exhibition.OrganizerId
                });
            });

            return exhibitionViewModels;
            //return new List<ExhibitionViewModel>() {
            //    new ExhibitionViewModel(){
            //        ExhibitionId = 999,
            //        Name = "test exhibition 01",
            //        Address = "here",
            //        Description = "test only",
            //        EndDate = "01/01/2001",
            //        Location = "there",
            //        Logo = "none",
            //        OrganizerId = "bill_gates",
            //        Qr = "asdfasdf",
            //        StartDate = "01/01/2000"
            //    }
            //};
        }

    }
}
