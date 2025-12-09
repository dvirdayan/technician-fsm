using System;
using FSM.Domain.Enums; // Fix: Updated Namespace

namespace FSM.Domain.Entities // Fix: Added 'FSM.'
{
    public class Task
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
        
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public TimeSpan Duration { get; set; } 

        public DateTime TimeWindowStart { get; set; } 

        public DateTime TimeWindowEnd { get; set; } 

        public TaskPriority Priority { get; set; } 

        public SkillSet RequiredSkills { get; set; } 

        public int? AssignedTechnicianId { get; set; }
        
        public virtual Technician AssignedTechnician { get; set; }

        public int SequenceIndex { get; set; } 

        public DateTime? ActualStartTime { get; set; } 
        
        public DateTime? ActualEndTime { get; set; }

        // Fix: Explicitly use the Enum to avoid confusion with System.Threading.Tasks.TaskStatus
        public FSM.Domain.Enums.TaskStatus Status { get; set; } = FSM.Domain.Enums.TaskStatus.Pending;

        public bool IsTimeInWindow(DateTime time)
        {
            return time >= TimeWindowStart && time <= TimeWindowEnd;
        }
    }
}