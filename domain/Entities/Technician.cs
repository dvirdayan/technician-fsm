using System;
using System.Collections.Generic;
using FSM.Domain.Enums; // Fix: Updated Namespace

namespace FSM.Domain.Entities
{
    public class Technician
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BaseAddress { get; set; }
        public double BaseLatitude { get; set; } 
        public double BaseLongitude { get; set; } 

        public SkillSet Skills { get; set; } 

        public TimeSpan ShiftStart { get; set; } 
        public TimeSpan ShiftEnd { get; set; } 
        
        public int MaxConcurrentTasks { get; set; } = 1; 
        public double EstimatedTravelSpeedKmH { get; set; } 
        
        public decimal HourlyCost { get; set; } 

        public virtual ICollection<TechnicianSchedule> Schedules { get; set; } = new List<TechnicianSchedule>();

        public bool HasSkill(SkillSet requiredSkill)
        {
            return (Skills & requiredSkill) == requiredSkill;
        }

        public TimeSpan GetShiftDuration()
        {
            return ShiftEnd - ShiftStart;
        }
    }
}