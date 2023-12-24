﻿using System.ComponentModel.DataAnnotations;

namespace LeisureReviewsAPI.Models
{
    public class AdditionalInfoModel
    {
        [Required]
        public string ExternalProvider { get; set; }

        [Required]
        public string ProviderKey { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
