using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseMangement.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseDuration { get; set; }
        public int CourseFee { get; set; }
        public string Language { get; set; }
        public string Prerequisite { get; set; }
        public string Description { get; set; }
        public string DeviceToBring { get; set; }
        public int Ratings { get; set; }
        public string Comments { get; set; }
        public DateTime CourseDate { get; set; }
        public DateTime CourseStartTime { get; set; }
        public DateTime CourseEndTime { get; set; }
        public string Venue { get; set; }
        public bool Status { get; set; }
        public string SuitableAgeGroup { get; set; }

        public int FkCourseCategoryId { get; set; }
        public CourseCategory CourseCategories { get; set; }
        public CourseSurvey CourseSurvey { get; set; }
    }
}
