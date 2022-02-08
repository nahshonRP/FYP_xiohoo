using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class SurveyUsers
    {
        [Key]
        public int Id { get; set; }
        public int FkSurveyId { get; set; }
        public int FkUserId { get; set; }

    }
}
