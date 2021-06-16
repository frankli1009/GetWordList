using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GetWords.Utilities;

namespace GetWords.Models
{
    public class WordRequirement
    {
        public WordRequirement()
        {
            Rejects = new List<string>();
        }

        [Required(ErrorMessage = "Please enter at least 3 letters available for the words")]
        [MinLength(length: 3, ErrorMessage = "Please enter at least 3 letters available for the words")]
        public string Letters { get; set; }
        [Required(ErrorMessage = "Please enter the length of the ")]
        [MinValue(3, ErrorMessage = "Target word length must be more than 2")]
        public int Length { get; set; }
        public string ExtraRegEx { get; set; }
        public List<string> Rejects { get; set; }
    }
}
