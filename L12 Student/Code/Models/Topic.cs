using System;
using System.Collections.Generic;

namespace Lesson12.Models
{
    public partial class Topic
    {
        public Topic()
        {
            Lesson = new HashSet<Lesson>();
        }

        public string TopicId { get; set; }
        public string Title { get; set; }
        public string ModuleId { get; set; }

        public virtual Module Module { get; set; }
        public virtual ICollection<Lesson> Lesson { get; set; }
    }
}
