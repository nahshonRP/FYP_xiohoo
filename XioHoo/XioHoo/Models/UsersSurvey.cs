using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class UsersSurvey
    {

        [Key]
        public int Id { get; set; }
        public int FkUserId { get; set; }
        public ApplicationUser User { get; set; }
        public int FkCourseSurveyId { get; set; }
        public CourseSurvey CourseSurvey { get; set; }

        public DateTime DateAdded { get; set; }
        public int Ratings { get; set; }
        public string Comments { get; set; }
        public List<UsersSurveyDetails> UsersSurveyDetails { get; set; }
    }
}
