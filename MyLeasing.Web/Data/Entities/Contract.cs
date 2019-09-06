﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MyLeasing.Web.Data.Entities
{
    public class Contract
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Remarks { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]

        public decimal Price { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]

        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]

        public DateTime EndDate { get; set; }

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:MM tt}", ApplyFormatInEditMode = true)]

        public DateTime StartDateLocal => StartDate.ToLocalTime();

        private int Required(object p)
        {
            throw new NotImplementedException();
        }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:MM tt}", ApplyFormatInEditMode = true)]
        public DateTime EndDateLocal => EndDate.ToLocalTime();

        public Owner Owner { get; set; }
        public Lessee Lessee { get; set; }
        public Property Property { get; set; }
    }
}
