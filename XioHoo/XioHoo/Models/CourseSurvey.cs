using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class CourseSurvey
    {
        [Key]
        public int Id { get; set; }
        public int FkCourceId { get; set; }
        public Course Course { get; set; }
        public int FkSurveyId { get; set; }
        public Survey Survey { get; set; }
        public List<UsersSurvey> UsersSurveys { get; set; }

    }
}
