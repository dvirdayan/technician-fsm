using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FSM.Application.Algorithms;
using FSM.Domain.Entities;
using FSM.Infrastructure.Persistence;

// Alias to prevent conflict
using TaskEntity = FSM.Domain.Entities.Task;

namespace FSM.Application.Services
{
    public class FsmService
    {
        private readonly JsonRepository<Technician> _techRepo;
        private readonly JsonRepository<TaskEntity> _taskRepo;
        
        private readonly GreedyScheduler _greedy;
        private readonly SimulatedAnnealingEngine _optimizer;
        private readonly WeightedObjectiveFunction _objective;

        public List<Technician> Technicians { get; private set; }
        public List<TaskEntity> Tasks { get; private set; }

        public FsmService()
        {
            _techRepo = new JsonRepository<Technician>("technicians.json");
            _taskRepo = new JsonRepository<TaskEntity>("tasks.json");

            _objective = new WeightedObjectiveFunction();
            _greedy = new GreedyScheduler();
            _optimizer = new SimulatedAnnealingEngine(_objective);

            LoadData();
        }

        public void LoadData()
        {
            Technicians = _techRepo.Load();
            Tasks = _taskRepo.Load();
            
            if (!Technicians.Any())
            {
                Technicians = FSM.Api.Program.GetDummyTechnicians();
                _techRepo.Save(Technicians);
            }
        }

        public void AddTask(TaskEntity task)
        {
            task.Id = Tasks.Any() ? Tasks.Max(t => t.Id) + 1 : 101;
            Tasks.Add(task);
            _taskRepo.Save(Tasks);
        }

        // FIX: Explicitly use System.Threading.Tasks.Task
        public async System.Threading.Tasks.Task<List<TechnicianSchedule>> RunOptimizationAsync()
        {
            Console.WriteLine("Generating Initial Schedule...");
            var initialSolution = _greedy.GenerateInitialSchedule(Technicians, Tasks);
            
            Console.WriteLine("Optimizing Routes...");
            var finalSolution = await _optimizer.OptimizeScheduleAsync(initialSolution, CancellationToken.None);
            
            return finalSolution;
        }
    }
}