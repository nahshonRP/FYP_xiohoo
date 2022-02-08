using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class Survey
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public string Comments { get; set; }
        public bool Status { get; set; }
        public DateTime DateCreated { get; set; }
        public CourseSurvey CourseSurvey { get; set; }
        public List<SurveyQuestions> SurveyQuestions { get; set; }
    }
}
