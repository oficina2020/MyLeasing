using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MyLeasing.Web.Helpers
{
    public interface ICombosHelpers
    {
        IEnumerable<SelectListItem> GetComboPropertyTypes();
        IEnumerable<SelectListItem> GetComboLessees();
    }
}