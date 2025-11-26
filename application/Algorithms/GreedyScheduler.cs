using System;
using System.Collections.Generic;
using System.Linq;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;

namespace FSM.Application.Algorithms
{
    public class GreedyScheduler : IInitialScheduler
    {
        public List<TechnicianSchedule> GenerateInitialSchedule(List<Technician> technicians, List<Task> tasks)
        {
            var schedules = InitializeSchedules(technicians);
            var unassignedTasks = new List<Task>();

            // Sort tasks: Urgent first, then by earliest deadline
            var sortedTasks = tasks
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.TimeWindowEnd)
                .ToList();

            foreach (var task in sortedTasks)
            {
                TechnicianSchedule bestSchedule = null;
                double bestScore = double.MaxValue; // Lower is better (minimizing distance/time)
                DateTime bestStartTime = DateTime.MaxValue;

                // Try to find the best technician for this task
                foreach (var schedule in schedules)
                {
                    var tech = schedule.Technician;
                    
                    // check skill
                    if (!tech.HasSkill(task.RequiredSkills)) continue;

                    // Time & Location Calculation
                    // Get the location/time of the last task in the schedule (or Base if empty)
                    var lastTask = schedule.Tasks.LastOrDefault();
                    
                    double startLat = lastTask?.Latitude ?? tech.BaseLatitude;
                    double startLon = lastTask?.Longitude ?? tech.BaseLongitude;
                    DateTime availableTime = lastTask?.ActualEndTime ?? DateTime.Today.Add(tech.ShiftStart);

                    // Estimate Travel (Euclidean for now - simpler than Map API for this stage)
                    double distanceKm = CalculateDistance(startLat, startLon, task.Latitude, task.Longitude);
                    double travelMinutes = (distanceKm / tech.EstimatedTravelSpeedKmH) * 60;
                    
                    DateTime arrivalTime = availableTime.AddMinutes(travelMinutes);
                    DateTime startTaskTime = arrivalTime;

                    // If we arrive before the window we wait.
                    if (arrivalTime < task.TimeWindowStart)
                    {
                        startTaskTime = task.TimeWindowStart;
                    }

                    DateTime finishTime = startTaskTime.Add(task.Duration);

                    // Shift End Constraint
                    DateTime shiftEnd = DateTime.Today.Add(tech.ShiftEnd);
                    if (finishTime > shiftEnd) continue;

                    // Window Constraint (Must start before window ends)
                    if (startTaskTime > task.TimeWindowEnd) continue;

                    // We want the tech who is closest and available earliest
                    double score = distanceKm; 

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestSchedule = schedule;
                        bestStartTime = startTaskTime;
                    }
                }

                // Assign the task if we found a valid technician
                if (bestSchedule != null)
                {
                    task.AssignedTechnicianId = bestSchedule.TechnicianId;
                    task.ActualStartTime = bestStartTime;
                    task.ActualEndTime = bestStartTime.Add(task.Duration);
                    task.SequenceIndex = bestSchedule.Tasks.Count + 1;
                    task.Status = TaskStatus.Scheduled;

                    bestSchedule.Tasks.Add(task);
                    
                    // Update schedule tracking stats
                    bestSchedule.TotalDistance += bestScore; // rough estimate
                }
                else
                {
                    unassignedTasks.Add(task); // Unscheduled
                }
            }

            return schedules;
        }

        private List<TechnicianSchedule> InitializeSchedules(List<Technician> technicians)
        {
            var list = new List<TechnicianSchedule>();
            foreach (var tech in technicians)
            {
                list.Add(new TechnicianSchedule
                {
                    TechnicianId = tech.Id,
                    Technician = tech,
                    Date = DateTime.Today,
                    Tasks = new List<Task>()
                });
            }
            return list;
        }

        // Simple Haversine or Euclidean distance helper
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Simplified Euclidean approximation for "local" distances
            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;
            return Math.Sqrt(dLat * dLat + dLon * dLon) * 111.0;
        }
    }
}   