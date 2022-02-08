using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class Questions
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SurveyQuestions> SurveyQuestions { get; set; }
    }
}
