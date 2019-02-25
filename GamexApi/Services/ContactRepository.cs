using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GamexApi.Models;
using Exception = System.Exception;

namespace GamexApi.Services {
    public class ContactRepository {
        private const string CacheKey = "ContactStore";

        public ContactRepository() {
            var context = HttpContext.Current;
            if (context != null) {
                if (context.Cache[CacheKey] == null) {
                    var contacts = new Contact[] {
                        new Contact {
                            Id = 1, Name = "Harry Potter"
                        },
                        new Contact {
                            Id = 2, Name = "Bill Gates"
                        }
                    };
                    context.Cache[CacheKey] = contacts;
                }
            }
        }

        public Contact[] GetAllContacts() {
            var context = HttpContext.Current;
            if (context != null) {
                return (Contact[]) context.Cache[CacheKey];
            }
            return new Contact[] {
                new Contact() {
                    Id = 0, Name = "Placeholder"
                }
            };
        }

        public bool SaveContact(Contact contact) {
            var context = HttpContext.Current;
            if (context != null) {
                try {
                    var currentData = ((Contact[]) context.Cache[CacheKey]).ToList();
                    currentData.Add(contact);
                    context.Cache[CacheKey] = currentData.ToArray();
                    return true;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            return false;
        }
    }
}