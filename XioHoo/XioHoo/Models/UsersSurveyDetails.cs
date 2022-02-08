using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class UsersSurveyDetails
    {
        [Key]
        public int Id { get; set; }

        public int FkQuestionID { get; set; }
        public string FkQuestionText { get; set; }
        public int AnswerScore { get; set; }
        public DateTime DateAdded { get; set; }

        public int FkUserSruveyID { get; set; }
        public UsersSurvey UsersSurvey { get; set; }
    }
}
