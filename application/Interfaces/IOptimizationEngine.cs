using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks; // Required for async/await
using FSM.Domain.Entities;

namespace FSM.Application.Interfaces
{
    /// <summary>
    /// Contract for the core optimization logic (Simulated Annealing / Evolutionary).
    /// </summary>
    public interface IOptimizationEngine
    {
        /// <summary>
        /// Takes an existing solution and attempts to improve its score iteratively.
        /// </summary>
        /// <param name="currentSchedule">The starting solution (e.g., from the Greedy alg).</param>
        /// <param name="cancellationToken">Allows stopping the process if the user cancels.</param>
        /// <returns>A significantly improved schedule.</returns>
        Task<List<TechnicianSchedule>> OptimizeScheduleAsync(
            List<TechnicianSchedule> currentSchedule, 
            CancellationToken cancellationToken);
    }
}