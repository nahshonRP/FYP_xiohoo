using System;
using System.Collections.Generic;

namespace Lesson12.Models
{
    public partial class Lesson
    {
        public string ModuleId { get; set; }
        public int LessonId { get; set; }
        public string TopicId { get; set; }
        public string Notes { get; set; }
        public string Attachments { get; set; }

        public virtual Module Module { get; set; }
        public virtual Topic Topic { get; set; }
    }
}
