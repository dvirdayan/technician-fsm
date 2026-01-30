using Microsoft.AspNetCore.Mvc;
using FSM.Application.Services;
using FSM.Domain.Entities;
using TaskEntity = FSM.Domain.Entities.Task;

namespace Fsm.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly FsmService _service;

        public TasksController()
        {
            // This reuses the EXACT same logic you built for the console!
            _service = new FsmService();
        }

        [HttpGet]
        public IEnumerable<TaskEntity> GetAll()
        {
            return _service.Tasks;
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> RunSchedule()
        {
            var result = await _service.RunOptimizationAsync();
            return Ok(result);
        }
    }
}