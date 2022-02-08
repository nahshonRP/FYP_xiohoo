using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models.ViewModels
{
    public class CourseSurveyViewModel
    {
        public int Id { get; set; }
        public int FkUserId { get; set; }
        public string Comment { get; set; }
        public int Ratings { get; set; }
        public int FkCourceId { get; set; }
        public Course Course { get; set; }
        public int FkSurveyId { get; set; }
        public Survey Survey { get; set; }
        public bool IsAlreadySubnitted { get; set; }
        public List<QuestionsViewModel> Questions { get; set; }
    }
}
