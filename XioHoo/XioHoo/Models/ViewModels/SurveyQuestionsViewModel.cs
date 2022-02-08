using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models.ViewModels
{
    public class SurveyQuestionsViewModel
    {
        public int Id { get; set; }
        public int FkSurveyId { get; set; }
        public int FkQuestionId { get; set; }
        public Survey Survey { get; set; }
        public List<QuestionsViewModel> Questions { get; set; }
    }
}
