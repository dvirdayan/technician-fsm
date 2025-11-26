using System;
using System.Collections.Generic;
using System.Linq;

namespace FSM.Domain.Entities
{
    public class TechnicianSchedule
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public virtual Technician Technician { get; set; } 

        // assingment matrix, 'A'
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

        // day of the schedule
        public DateTime Date { get; set; } 
        
        // distance for w1
        public double TotalDistance { get; set; } 

        // delay for w2
        public double TotalDelay { get; set; } 

        // Is the schedule currently valid? 
        public bool IsFeasible { get; set; } = true;


        // sequence of the tasks, 'S'
        public List<Task> GetOrderedRoute()
        {
            return Tasks.OrderBy(t => t.SequenceIndex).ToList();
        }

        /// start times, 'R'
        public Dictionary<int, DateTime?> GetStartTimes()
        {
            return Tasks.ToDictionary(t => t.Id, t => t.ActualStartTime);
        }
    }
}