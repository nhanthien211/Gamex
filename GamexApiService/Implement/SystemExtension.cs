using System.Collections.Generic;
using System.Linq;
using GamexApiService.Models;
using Newtonsoft.Json;

namespace GamexApiService.Implement {
    public static class SystemExtension {
        public static T Clone<T>(this T source) {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static List<CompanyShortViewModel> RemoveRedundancies(this List<CompanyShortViewModel> list) {
            var dict = new Dictionary<string, CompanyShortViewModel>();
            list.ForEach(company => {
                if (!dict.ContainsKey(company.CompanyId)) {
                    dict.Add(company.CompanyId, company);
                }
            });
            return dict.Values.ToList();
        }
    }
}