using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Algorithms
{
    public class SimulatedAnnealingEngine : IOptimizationEngine
    {
        private readonly IAlgorithmGoal _objectiveFunction;
        private readonly Random _random = new Random();

        // SA Parameters
        private double _temperature = 1000.0;
        private readonly double _coolingRate = 0.995;
        private readonly double _minTemperature = 0.1;

        public SimulatedAnnealingEngine(IAlgorithmGoal objectiveFunction)
        {
            _objectiveFunction = objectiveFunction;
        }

        public async Task<List<TechnicianSchedule>> OptimizeScheduleAsync(
            List<TechnicianSchedule> currentSchedule, 
            CancellationToken cancellationToken)
        {
            // Clone the initial solution so we don't mutate the original reference
            var currentSolution = CloneSolution(currentSchedule);
            var bestSolution = CloneSolution(currentSolution);

            double currentScore = _objectiveFunction.CalculateScore(currentSolution);
            double bestScore = currentScore;

            // Main SA Loop
            while (_temperature > _minTemperature && !cancellationToken.IsCancellationRequested)
            {
                // Create a neighbor solution (Mutate)
                var neighborSolution = CloneSolution(currentSolution);
                
                // This now performs Smart Mutation (checking skills)
                ApplySmartMutation(neighborSolution);

                // Calculate new score
                double neighborScore = _objectiveFunction.CalculateScore(neighborSolution);

                // Acceptance Probability
                if (AcceptanceProbability(currentScore, neighborScore, _temperature) > _random.NextDouble())
                {
                    currentSolution = neighborSolution;
                    currentScore = neighborScore;

                    // Keep track of the absolute best we've seen
                    if (currentScore < bestScore)
                    {
                        bestSolution = CloneSolution(currentSolution);
                        bestScore = currentScore;
                    }
                }

                // Cool down
                _temperature *= _coolingRate;
                
                // Yield control periodically to ensure the application doesn't freeze
                if (_temperature % 10 < 1) await System.Threading.Tasks.Task.Yield();
            }

            return bestSolution;
        }

        private double AcceptanceProbability(double currentScore, double neighborScore, double temp)
        {
            // If neighbor is better (lower cost), always accept
            if (neighborScore < currentScore) return 1.0;

            // If neighbor is worse, accept with probability exp(-(new - old) / temp)
            return Math.Exp(-(neighborScore - currentScore) / temp);
        }

        /// Tries to improve the schedule by either moving a task to a qualified technician
        /// or re-ordering tasks within a single technician's route.
        private void ApplySmartMutation(List<TechnicianSchedule> solution)
        {
            // 50% chance to Move a Task, 50% chance to Swap Order inside a route
            if (_random.NextDouble() > 0.5)
            {
                MoveTaskToQualifiedTechnician(solution);
            }
            else
            {
                SwapTaskOrderInternal(solution);
            }
        }

        private void MoveTaskToQualifiedTechnician(List<TechnicianSchedule> solution)
        {
            // Pick a random technician who actually has tasks
            var techWithTasks = solution.Where(s => s.Tasks.Count > 0).OrderBy(x => _random.Next()).FirstOrDefault();
            if (techWithTasks == null) return;

            // Pick a random task from them
            var taskToMove = techWithTasks.Tasks.ToList()[_random.Next(techWithTasks.Tasks.Count)];

            // Find VALID targets: Technicians who have the required skill 
            // AND are not the current technician
            var validTargets = solution
                .Where(s => s.TechnicianId != techWithTasks.TechnicianId)
                .Where(s => s.Technician.HasSkill(taskToMove.RequiredSkills))
                .ToList();

            // If no one else can do this job, we can't move it. 
            // Fallback to internal swapping so we don't waste the iteration.
            if (validTargets.Count == 0)
            {
                SwapTaskOrderInternal(solution);
                return;
            }

            // Move the task to a random VALID technician
            var targetSchedule = validTargets[_random.Next(validTargets.Count)];

            techWithTasks.Tasks.Remove(taskToMove);
            targetSchedule.Tasks.Add(taskToMove);

            // Update the task ownership
            taskToMove.AssignedTechnicianId = targetSchedule.TechnicianId;
            taskToMove.AssignedTechnician = targetSchedule.Technician; 
        }

        private void SwapTaskOrderInternal(List<TechnicianSchedule> solution)
        {
            // Pick a technician with at least 2 tasks
            var candidate = solution.Where(s => s.Tasks.Count >= 2).OrderBy(x => _random.Next()).FirstOrDefault();
            if (candidate == null) return;

            var tasks = candidate.Tasks.ToList();
            int indexA = _random.Next(tasks.Count);
            int indexB = _random.Next(tasks.Count);

            // Ensure we picked different indices
            while (indexA == indexB)
            {
                indexB = _random.Next(tasks.Count);
            }

            // Swap sequence indices to change the route order
            int tempSeq = tasks[indexA].SequenceIndex;
            tasks[indexA].SequenceIndex = tasks[indexB].SequenceIndex;
            tasks[indexB].SequenceIndex = tempSeq;
            
            // (Note: The calling code or ObjectiveFunction usually re-sorts by SequenceIndex 
            // before calculating travel time, or we can physical swap them in the list here)
        }

        // Deep copy helper
        private List<TechnicianSchedule> CloneSolution(List<TechnicianSchedule> source)
        {
            var newSolution = new List<TechnicianSchedule>();
            foreach (var s in source)
            {
                // We create a new Schedule object
                var newSch = new TechnicianSchedule
                {
                    Id = s.Id,
                    TechnicianId = s.TechnicianId,
                    Technician = s.Technician, // Reference copy of the Tech entity is fine (it's static data)
                    Date = s.Date,
                    TotalDistance = s.TotalDistance,
                    // IMPORTANT: We must create a NEW List for the tasks so we can add/remove
                    // without affecting the original solution.
                    Tasks = new List<FSM.Domain.Entities.Task>(s.Tasks) 
                };
                newSolution.Add(newSch);
            }
            return newSolution;
        }
    }
}