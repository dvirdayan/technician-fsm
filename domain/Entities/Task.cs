using System;
using Domain.Enums; // Assuming you have your enums here

namespace Domain.Entities
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

        // Bitwise enum to check if technician has matching skills
        public SkillSet RequiredSkills { get; set; } 

        public int? AssignedTechnicianId { get; set; }
        
        // Navigation property for EF Core (optional but recommended)
        public virtual Technician AssignedTechnician { get; set; }

        public int SequenceIndex { get; set; } 

        public DateTime? ActualStartTime { get; set; } 
        
        public DateTime? ActualEndTime { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public bool IsTimeInWindow(DateTime time)
        {
            return time >= TimeWindowStart && time <= TimeWindowEnd;
        }
    }
}