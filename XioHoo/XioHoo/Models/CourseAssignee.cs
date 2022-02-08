using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class CourseAssignee
    {
        [Key]
        public int Id { get; set; }
        public int fkCourseId { get; set; }
        public int fkUserId { get; set; }

    }
}
