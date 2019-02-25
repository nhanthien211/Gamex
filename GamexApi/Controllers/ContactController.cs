using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GamexApi.Models;
using GamexApi.Services;

namespace GamexApi.Controllers {
    public class ContactController : ApiController {
        private ContactRepository contactRepository;

        public ContactController() {
            this.contactRepository = new ContactRepository();
        }

        public Contact[] Get() {
            return contactRepository.GetAllContacts();
        }

        public HttpResponseMessage Post(Contact contact) {
            this.contactRepository.SaveContact(contact);
            var response = Request.CreateResponse(HttpStatusCode.Created, contact);
            return response;
        }
    }
}
