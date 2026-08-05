using Microsoft.AspNetCore.Mvc;
using TaskManagement.Dtos;
using TaskManagement.Interfaces;
using TaskManagement.Helpers;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeeksController : ControllerBase
    {
        private readonly IWeekService _weekService;

        public WeeksController(IWeekService weekService)
        {
            _weekService = weekService;
        }

        // --- 1. إنشاء أسبوع جديد ---
        [HttpPost]
        public async Task<IActionResult> CreateWeek(
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId,
            [FromBody] WeekCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var result = await _weekService.CreateWeekAsync(dto, validId);
            return CreatedAtAction(nameof(GetWeekById), new { id = result.Id }, result);
        }

        // --- 2. جلب جميع الأسابيع ---
        [HttpGet]
        public async Task<IActionResult> GetAllWeeks(
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId)
        {
            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var weeks = await _weekService.GetAllWeeksAsync(validId);
            return Ok(weeks);
        }

        // --- 3. جلب أسبوع محدد (تمت إزالة try-catch، المعالج العالمي سيتعامل مع KeyNotFoundException) ---
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeekById(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId)
        {
            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var week = await _weekService.GetWeekByIdAsync(id, validId);
            return Ok(week);
        }

        // --- 4. جلب إحصائيات مرآة الأسبوع (تمت إزالة try-catch) ---
        [HttpGet("{id}/insights")]
        public async Task<IActionResult> GetWeekInsights(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId)
        {
            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var insights = await _weekService.GetWeekInsightsAsync(id, validId);
            return Ok(insights);
        }

        // --- 5. تحديث أسبوع ---
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeek(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId,
            [FromBody] WeekCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var result = await _weekService.UpdateWeekAsync(id, dto, validId);

            if (!result)
                return NotFound(new { message = $"الأسبوع رقم {id} غير موجود." });

            return NoContent();
        }

        // --- 6. حذف أسبوع ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeek(
            int id,
            [FromHeader(Name = "X-Anonymous-Id")] string? anonymousId)
        {
            var validId = AnonymousIdHelper.GetOrGenerate(anonymousId);
            var result = await _weekService.DeleteWeekAsync(id, validId);

            if (!result)
                return NotFound(new { message = $"الأسبوع رقم {id} غير موجود." });

            return NoContent();
        }
    }
}