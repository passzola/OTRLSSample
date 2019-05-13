﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CUSTOR.OTRLS.Core
{
    public partial class Lookup
    {
        public int LookupId { get; set; }
        public int LookUpTypeId { get; set; }
        public string English { get; set; }
        public string Amharic { get; set; }
        public string Tigrigna { get; set; }
        public string AfanOromo { get; set; }
        public string Afar { get; set; }
        public string Somali { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual LookUpType LookUpType { get; set; }
    }
    public partial class LookupDTO
    {
        public LookupDTO()
        {
        }

        [Key]
        public int LookupId { get; set; }
        public int LookUpTypeId { get; set; }
        public string Description { get; set; }
    }
}
