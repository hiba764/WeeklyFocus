using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Dtos;
using TaskManagement.Interfaces;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    // هذه الخدمة تنفذ جميع العمليات المتعلقة بالأسابيع (IWeekService)
    // وهي القلب النابض لحساب الإنتاجية وتحليل التسويف
    public class WeekService : IWeekService
    {
        // ربط DbContext للتعامل مع قاعدة البيانات
        private readonly AppDbContext _context;

        // المُنشئ: يحقن (inject) DbContext عن طريق Dependency Injection
        public WeekService(AppDbContext context)
        {
            _context = context;
        }

        // --- الدالة المساعدة (خاصة): تحويل Week + مهامها إلى WeekReadDto ---
        // هذه الدالة تقوم بالتحويل اليدوي (Manual Mapping) لأننا لم نستخدم AutoMapper
        private WeekReadDto MapToReadDto(Week week)
        {
            // حساب عدد المهام الكلي (مع تجاهل المهام المحذوفة Soft Delete)
            var totalTasks = week.Tasks.Count(t => !t.IsDeleted);

            // حساب عدد المهام المنجزة
            var completedTasks = week.Tasks.Count(t => !t.IsDeleted && t.Status == Enums.TaskItemStatus.Completed);

            // حساب النسبة المئوية (إذا كان العدد الكلي صفراً، النسبة = 0)
            double percentage = totalTasks == 0 ? 0 : (double)completedTasks / totalTasks * 100;

            // حساب نقاط الخبرة (XP): مجموع (الوقت المتوقع * معامل الصعوبة) للمهام المنجزة فقط
            // معامل الصعوبة: Easy=1, Medium=2, Hard=3 (وهي قيم Enum)
            var xp = week.Tasks
                .Where(t => !t.IsDeleted && t.Status == Enums.TaskItemStatus.Completed)
                .Sum(t => t.ExpectedMinutes * (int)t.Difficulty);

            return new WeekReadDto
            {
                Id = week.Id,
                StartDate = week.StartDate,
                EndDate = week.EndDate,
                GoalPercentage = Math.Round(percentage, 2), // تقريب النسبة لرقمين عشريين
                TasksCount = totalTasks,
                CompletedTasks = completedTasks,
                TotalXp = xp,
                CreatedAt = week.CreatedAt
            };
        }

        // 1. جلب جميع الأسابيع لمستخدم معين (مع تجاهل المحذوفة)
        public async Task<IEnumerable<WeekReadDto>> GetAllWeeksAsync(string anonymousId)
        {
            // جلب الأسابيع من قاعدة البيانات مع تحميل المهام المرتبطة بها (Include)
            var weeks = await _context.Weeks
                .Include(w => w.Tasks)
                .Where(w => w.AnonymousId == anonymousId && !w.IsDeleted)
                .OrderByDescending(w => w.StartDate) // ترتيب تنازلي (الأحدث أولاً)
                .ToListAsync();

            // تحويل كل أسبوع إلى DTO باستخدام الدالة المساعدة
            return weeks.Select(MapToReadDto);
        }

        // 2. جلب أسبوع محدد بواسطة ID (مع التأكد من أنه يخص المستخدم الحالي)
        public async Task<WeekReadDto> GetWeekByIdAsync(int id, string anonymousId)
        {
            var week = await _context.Weeks
                .Include(w => w.Tasks)
                .FirstOrDefaultAsync(w => w.Id == id && w.AnonymousId == anonymousId && !w.IsDeleted);

            // إذا لم يتم العثور على الأسبوع، نرمي خطأ (سيتعامل معه الـ Global Exception Handler لاحقاً)
            if (week == null)
                throw new KeyNotFoundException($"الأسبوع رقم {id} غير موجود أو لا يخص هذا المستخدم.");

            return MapToReadDto(week);
        }

        // 3. إنشاء أسبوع جديد
        public async Task<WeekReadDto> CreateWeekAsync(WeekCreateDto dto, string anonymousId)
        {
            // التحقق من صحة التواريخ (قاعدة العمل الأساسية)
            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("يجب أن يكون تاريخ النهاية أكبر من تاريخ البداية.");

            // إنشاء كائن Week جديد من البيانات المرسلة
            var week = new Week
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                AnonymousId = anonymousId, // ربط الأسبوع بالمستخدم الحالي
                CreatedAt = DateTime.UtcNow
            };

            // إضافة الأسبوع إلى قاعدة البيانات
            await _context.Weeks.AddAsync(week);
            await _context.SaveChangesAsync();

            // إعادة عرض الأسبوع الذي تم إنشاؤه (محول إلى DTO)
            return MapToReadDto(week);
        }

        // 4. تحديث أسبوع (التواريخ فقط)
        public async Task<bool> UpdateWeekAsync(int id, WeekCreateDto dto, string anonymousId)
        {
            // البحث عن الأسبوع مع التأكد من أنه يخص المستخدم
            var week = await _context.Weeks
                .FirstOrDefaultAsync(w => w.Id == id && w.AnonymousId == anonymousId && !w.IsDeleted);

            if (week == null)
                return false;

            // التحقق من صحة التواريخ الجديدة
            if (dto.EndDate <= dto.StartDate)
                throw new ArgumentException("يجب أن يكون تاريخ النهاية أكبر من تاريخ البداية.");

            // تحديث التواريخ
            week.StartDate = dto.StartDate;
            week.EndDate = dto.EndDate;

            await _context.SaveChangesAsync();
            return true;
        }

        // 5. حذف أسبوع (حذف ناعم - Soft Delete)
        public async Task<bool> DeleteWeekAsync(int id, string anonymousId)
        {
            var week = await _context.Weeks
                .FirstOrDefaultAsync(w => w.Id == id && w.AnonymousId == anonymousId && !w.IsDeleted);

            if (week == null)
                return false;

            // الحذف الناعم: فقط نغير القيمة إلى true
            week.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // 6. الميزة الذكية: جلب بيانات "مرآة الأسبوع" (الشاشة الخامسة)
        public async Task<WeekInsightsDto> GetWeekInsightsAsync(int id, string anonymousId)
        {
            // جلب الأسبوع مع جميع مهامه وبيانات أسباب الفشل
            var week = await _context.Weeks
                .Include(w => w.Tasks)
                    .ThenInclude(t => t.FailureReason) // تحميل سبب الفشل لكل مهمة
                .FirstOrDefaultAsync(w => w.Id == id && w.AnonymousId == anonymousId && !w.IsDeleted);

            if (week == null)
                throw new KeyNotFoundException($"الأسبوع رقم {id} غير موجود.");

            // تصفية المهام الغير محذوفة
            var tasks = week.Tasks.Where(t => !t.IsDeleted).ToList();
            var totalTasks = tasks.Count;
            var completedTasks = tasks.Count(t => t.Status == Enums.TaskItemStatus.Completed);
            var failedTasks = tasks.Count(t => t.Status == Enums.TaskItemStatus.Failed);

            // حساب النسبة المئوية
            double percentage = totalTasks == 0 ? 0 : (double)completedTasks / totalTasks * 100;

            // حساب النقاط (XP) للمهام المنجزة
            var xp = tasks
                .Where(t => t.Status == Enums.TaskItemStatus.Completed)
                .Sum(t => t.ExpectedMinutes * (int)t.Difficulty);

            // --- تحليل السبب الأكثر تكراراً للفشل ---
            // نجمع جميع المهام الفاشلة التي لها سبب محدد
            var failedWithReason = tasks
                .Where(t => t.Status == Enums.TaskItemStatus.Failed && t.FailureReasonId.HasValue)
                .ToList();

            // نبحث عن السبب الأكثر تكراراً عن طريق التجميع (Group By)
            var topFailure = failedWithReason
                .GroupBy(t => t.FailureReason!.Reason) // التجميع حسب النص
                .OrderByDescending(g => g.Count())      // ترتيب تنازلي حسب العدد
                .Select(g => new { Reason = g.Key, Count = g.Count() })
                .FirstOrDefault();

            // --- العثور على أصعب مهمة تم إنجازها ---
            var hardestCompleted = tasks
                .Where(t => t.Status == Enums.TaskItemStatus.Completed && t.Difficulty == Enums.DifficultyLevel.Hard)
                .OrderByDescending(t => t.ExpectedMinutes) // نأخذ الأطول وقتاً كـ "أصعب"
                .Select(t => t.Title)
                .FirstOrDefault();

            // --- توليد رسالة تحفيزية بناءً على نسبة الإنجاز ---
            string message;
            if (percentage >= 80)
                message = "👏 ممتاز! استمر على هذا المنوال المذهل. أنت حقاً منجز!";
            else if (percentage >= 50)
                message = "💪 جيد جداً! حاول الأسبوع القادم التركيز على المهام الصعبة في الصباح الباكر.";
            else
                message = "🧐 لا بأس، كل أسبوع هو فرصة جديدة. حاول تقليل عدد المهام اليومية وركز على الجودة بدلاً من الكمية.";

            // إنشاء كائن الـ DTO وإرجاعه
            return new WeekInsightsDto
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                CompletionPercentage = Math.Round(percentage, 2),
                TotalXp = xp,
                TopFailureReason = topFailure?.Reason ?? "لا يوجد أسباب فشل مسجلة", // إذا لم يوجد أي سبب
                TopFailureReasonCount = topFailure?.Count ?? 0,
                HardestCompletedTask = hardestCompleted ?? "لا توجد مهام صعبة مكتملة",
                MotivationalMessage = message
            };
        }
    }
}