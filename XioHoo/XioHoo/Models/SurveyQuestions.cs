using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class SurveyQuestions
    {
        [Key]
        public int Id { get; set; }
        public int FkQuestionId { get; set; }
        public Questions Question { get; set; }
        public int FkSurveyId { get; set; }
        public Survey Survey { get; set; }
    }
}
