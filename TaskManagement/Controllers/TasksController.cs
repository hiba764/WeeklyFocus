using Microsoft.AspNetCore.Mvc;
using TaskManagement.Dtos;
using TaskManagement.Interfaces;
using TaskManagement.Helpers;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // --- 1. إضافة مهمة جديدة ---
        [HttpPost]
        public async Task<IActionResult> CreateTask(
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId,
            [FromBody] TaskCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var result = await _taskService.CreateTaskAsync(dto, validId);
            return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
        }

        // --- 2. جلب جميع المهام لأسبوع معين (تمت إزالة try-catch) ---
        [HttpGet]
        public async Task<IActionResult> GetTasksByWeek(
            [FromQuery] int weekId,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId)
        {
            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var tasks = await _taskService.GetTasksByWeekAsync(weekId, validId);
            return Ok(tasks);
        }

        // --- 3. جلب مهام اليوم (تمت إزالة try-catch) ---
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayTasks(
            [FromQuery] int weekId,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId)
        {
            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var tasks = await _taskService.GetTodayTasksAsync(weekId, validId);
            return Ok(tasks);
        }

        // --- 4. جلب مهمة محددة (تمت إزالة try-catch) ---
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId)
        {
            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var task = await _taskService.GetTaskByIdAsync(id, validId);
            return Ok(task);
        }

        // --- 5. تحديث مهمة ---
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId,
            [FromBody] TaskUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var result = await _taskService.UpdateTaskAsync(id, dto, validId);

            if (!result)
                return NotFound(new { message = $"المهمة رقم {id} غير موجودة." });

            return NoContent();
        }

        // --- 6. حذف مهمة ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId)
        {
            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var result = await _taskService.DeleteTaskAsync(id, validId);

            if (!result)
                return NotFound(new { message = $"المهمة رقم {id} غير موجودة." });

            return NoContent();
        }

        // --- 7. إنهاء المهمة بنجاح (تمت إزالة try-catch لـ InvalidOperationException) ---
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteTask(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId,
            [FromBody] CompleteTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var result = await _taskService.CompleteTaskAsync(id, dto, validId);

            if (!result)
                return NotFound(new { message = $"المهمة رقم {id} غير موجودة." });

            return Ok(new { message = "🎉 أحسنت! تم إنجاز المهمة بنجاح." });
        }

        // --- 8. فشل المهمة (تمت إزالة try-catch لـ KeyNotFoundException) ---
        [HttpPatch("{id}/fail")]
        public async Task<IActionResult> FailTask(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId,
            [FromBody] FailTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var result = await _taskService.FailTaskAsync(id, dto.FailureReasonId, dto.Note, validId);

            if (!result)
                return NotFound(new { message = $"المهمة رقم {id} غير موجودة." });

            return Ok(new { message = "تم تسجيل سبب الفشل بنجاح." });
        }
    }
}