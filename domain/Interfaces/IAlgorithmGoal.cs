using System.Collections.Generic;
using Domain.Entities; // TechnicianSchedule

namespace FSM.Domain.Entities
{
    public interface IAlgorithmGoal
    {
        /// <summary>
        /// Calculates the "fitness" or "score" of a complete solution.
        /// Higher is usually better (Maximization), or Lower is better (Minimization).
        /// Based on your formula: f(s) = w1(-travel) + w2(-delay) + w3(urgent) + w4(-overtime)
        /// </summary>
        /// <param name="solution">The list of schedules for all technicians.</param>
        /// <returns>A numerical score representing quality.</returns>
        double CalculateScore(IEnumerable<TechnicianSchedule> solution);

        /// <summary>
        /// Allows retrieving the specific component scores for reporting/debugging.
        /// e.g., "This schedule is good because travel is low, but bad because overtime is high."
        /// </summary>
        ScoreBreakdown GetDetailedScore(IEnumerable<TechnicianSchedule> solution);
    }

    /// <summary>
    /// A simple DTO to hold the individual components of the score.
    /// Helpful for the Dashboard to show *why* a schedule was chosen.
    /// </summary>
    public class ScoreBreakdown
    {
        public double TotalTravelTime { get; set; }
        public double TotalDelay { get; set; }
        public int CompletedUrgentTasks { get; set; }
        public double OvertimeHours { get; set; }
        
        public double FinalWeightedScore { get; set; }
    }
}