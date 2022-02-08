using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models.ViewModels
{
    public class CourseAttendanceViewModel
    {
        public int Id { get; set; }
        public int FkCourseId { get; set; }
        public int FkUserId { get; set; }
        public DateTime AttendaceDateTime { get; set; }
        public Course Course { get; set; }
        public List<ApplicationUser> Users { get; set; }

    }
}
