using System;
using System.Collections.Generic;

namespace Lesson12.Models
{
    public partial class Module
    {
        public Module()
        {
            Lesson = new HashSet<Lesson>();
            Topic = new HashSet<Topic>();
        }

        public string ModuleId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Lesson> Lesson { get; set; }
        public virtual ICollection<Topic> Topic { get; set; }
    }
}
