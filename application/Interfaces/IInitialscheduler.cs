using System.Collections.Generic;
using FSM.Domain.Entities;

namespace FSM.Application.Interfaces
{
    /// <summary>
    /// Contract for generating a fast, valid initial solution.
    /// Usually implemented via Greedy heuristics.
    /// </summary>
    public interface IInitialScheduler
    {
        /// <summary>
        /// Takes available resources and tasks, and produces a baseline schedule.
        /// This should run very quickly.
        /// </summary>
        /// <param name="technicians">The list of available field agents.</param>
        /// <param name="tasks">The backlog of work to be done.</param>
        /// <returns>A list of schedules (routes) for each technician.</returns>
        List<TechnicianSchedule> GenerateInitialSchedule(List<Technician> technicians, List<Task> tasks);
    }
}