﻿using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Models;
using System.Threading.Tasks;

namespace MyLeasing.Web.Helpers
{
    public interface IConverterHelper
    {
        Task<Property> toPropertyAsync(PropertyViewModel model, bool isNew);

        PropertyViewModel ToPropertyViewModel(Property property);

        Task<Contract> ToContractAsync(ContractViewModel model, bool isNew);

        ContractViewModel ToContractViewModel(Contract contract);
    }
}