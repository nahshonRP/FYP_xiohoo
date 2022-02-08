using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models.ViewModels
{
    public class QuestionsViewModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}
