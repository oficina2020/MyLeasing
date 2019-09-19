using Microsoft.AspNetCore.Mvc.Rendering;
using MyLeasing.Web.Data;
using System.Collections.Generic;
using System.Linq;

namespace MyLeasing.Web.Helpers
{
    public class CombosHelpers : ICombosHelpers
    {
        private readonly DataContext _dataContext;

        public CombosHelpers(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<SelectListItem> GetComboPropertyTypes()
        {
            List<SelectListItem> list = _dataContext.PropertyTypes.Select(pt => new SelectListItem
            {
                Text = pt.Name,
                Value = pt.Id.ToString(),
            }
            ).OrderBy(orden => orden.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Select a propertype",
                Value = "0",
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboLessees()
        {
            List<SelectListItem> list = _dataContext.Lessees.Select(arrendatario => new SelectListItem
            {
                Text = arrendatario.User.FullNameWithDocument,
                Value = arrendatario.Id.ToString(),
            }
            ).OrderBy(orden => orden.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Select a lessee",
                Value = "0",
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboRoles()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "(Select a role...)" },
                new SelectListItem { Value = "1", Text = "Lessee (Arrendatario)" },
                new SelectListItem { Value = "2", Text = "Owner (Propietario" }
            };

            return list;
        }
    }
}