using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Dtos;
using TaskManagement.Enums;
using TaskManagement.Interfaces;
using TaskManagement.Models;

namespace TaskManagement.Services
{


    // هذه الخدمة تنفذ جميع العمليات المتعلقة بالمهام (ITaskService)
    // مسؤولة عن إنشاء المهام، تعديلها، تغيير حالتها (نجاح/فشل)، وتصفية مهام اليوم
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        // --- الدالة المساعدة (خاصة): تحويل TaskItem إلى TaskReadDto ---
        // تقوم بتحويل الأرقام (Enums) إلى نصوص مفهومة للواجهة الأمامية
        private TaskReadDto MapToReadDto(TaskItem task)
        {
            // تحويل رقم الأولوية (1,2,3) إلى نص (منخفضة، متوسطة، عالية)
            string priorityText = task.Priority switch
            {
                PriorityLevel.Low => "منخفضة",
                PriorityLevel.Medium => "متوسطة",
                PriorityLevel.High => "عالية",
                _ => "غير معروف"
            };

            // تحويل رقم الصعوبة (1,2,3) إلى نص
            string difficultyText = task.Difficulty switch
            {
                DifficultyLevel.Easy => "سهلة",
                DifficultyLevel.Medium => "متوسطة",
                DifficultyLevel.Hard => "صعبة",
                _ => "غير معروف"
            };

            // تحويل رقم الحالة (0,1,2) إلى نص
            string statusText = task.Status switch
            {
                TaskItemStatus.Waiting => "في الانتظار",
                TaskItemStatus.Completed => "منجزة ✅",
                TaskItemStatus.Failed => "فاشلة ❌",
                _ => "غير معروف"
            };

            return new TaskReadDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Priority = priorityText,
                Difficulty = difficultyText,
                ExpectedMinutes = task.ExpectedMinutes,
                ActualMinutes = task.ActualMinutes,
                DueDate = task.DueDate,
                Status = statusText,
                Note = task.Note,
                ImageUrl = task.ImageUrl,
                WeekStartDate = task.Week.StartDate, // نجلب تاريخ الأسبوع من العلاقة
                FailureReason = task.FailureReason?.Reason, // اسم سبب الفشل (إن وجد)
                CreatedAt = task.CreatedAt
            };
        }

        // 1. جلب جميع مهام أسبوع معين
        public async Task<IEnumerable<TaskReadDto>> GetTasksByWeekAsync(int weekId, string anonymousId)
        {
            // نتأكد أولاً أن الأسبوع يخص المستخدم الحالي
            var week = await _context.Weeks
                .FirstOrDefaultAsync(w => w.Id == weekId && w.AnonymousId == anonymousId && !w.IsDeleted);

            if (week == null)
                throw new KeyNotFoundException("الأسبوع غير موجود أو لا يخص هذا المستخدم.");

            // جلب المهام المرتبطة بهذا الأسبوع والتي لم تُحذف
            var tasks = await _context.TaskItems
                .Include(t => t.Week)
                .Include(t => t.FailureReason)
                .Where(t => t.WeekId == weekId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt) // الأحدث أولاً
                .ToListAsync();

            return tasks.Select(MapToReadDto);
        }

        // 2. **مهام اليوم**: المهام التي في الانتظار وتاريخ استحقاقها هو اليوم
        public async Task<IEnumerable<TaskReadDto>> GetTodayTasksAsync(int weekId, string anonymousId)
        {
            var today = DateTime.Today; // تاريخ اليوم (بدون وقت)

            var tasks = await _context.TaskItems
                .Include(t => t.Week)
                .Include(t => t.FailureReason)
                .Where(t => t.WeekId == weekId &&
                            t.Week.AnonymousId == anonymousId && // التأكد من المستخدم
                            !t.IsDeleted &&
                            t.Status == TaskItemStatus.Waiting &&
                            t.DueDate != null &&
                            t.DueDate.Value.Date == today) // مقارنة التاريخ فقط
                .OrderBy(t => t.DueDate) // الترتيب حسب الموعد الأقرب
                .ToListAsync();

            return tasks.Select(MapToReadDto);
        }

        // 3. جلب مهمة محددة بواسطة ID (لعرض التفاصيل)
        public async Task<TaskReadDto> GetTaskByIdAsync(int id, string anonymousId)
        {
            var task = await _context.TaskItems
                .Include(t => t.Week)
                .Include(t => t.FailureReason)
                .FirstOrDefaultAsync(t => t.Id == id && t.Week.AnonymousId == anonymousId && !t.IsDeleted);

            if (task == null)
                throw new KeyNotFoundException("المهمة غير موجودة أو لا تخص هذا المستخدم.");

            return MapToReadDto(task);
        }

        // 4. إنشاء مهمة جديدة
        public async Task<TaskReadDto> CreateTaskAsync(TaskCreateDto dto, string anonymousId)
        {
            // التحقق من وجود الأسبوع وأنه يخص المستخدم
            var week = await _context.Weeks
                .FirstOrDefaultAsync(w => w.Id == dto.WeekId && w.AnonymousId == anonymousId && !w.IsDeleted);

            if (week == null)
                throw new KeyNotFoundException("الأسبوع غير موجود أو لا يخص هذا المستخدم.");

            // تحويل الأرقام المرسلة من DTO إلى الـ Enum المناسب
            var priority = (PriorityLevel)dto.Priority;
            var difficulty = (DifficultyLevel)dto.Difficulty;

            // إنشاء كائن المهمة
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = priority,
                Difficulty = difficulty,
                ExpectedMinutes = dto.ExpectedMinutes,
                DueDate = dto.DueDate,
                WeekId = dto.WeekId,
                Status = TaskItemStatus.Waiting, // الحالة الافتراضية دائماً "في الانتظار"
                CreatedAt = DateTime.UtcNow
            };

            await _context.TaskItems.AddAsync(task);
            await _context.SaveChangesAsync();

            // إعادة تحميل المهمة مع العلاقات لعرضها بشكل كامل
            var createdTask = await _context.TaskItems
                .Include(t => t.Week)
                .Include(t => t.FailureReason)
                .FirstAsync(t => t.Id == task.Id);

            return MapToReadDto(createdTask);
        }

        // 5. تحديث مهمة موجودة (جميع الحقول عدا WeekId)
        public async Task<bool> UpdateTaskAsync(int id, TaskUpdateDto dto, string anonymousId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.Week.AnonymousId == anonymousId && !t.IsDeleted);

            if (task == null)
                return false;

            // تحديث جميع الحقول القابلة للتعديل
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = (PriorityLevel)dto.Priority;
            task.Difficulty = (DifficultyLevel)dto.Difficulty;
            task.ExpectedMinutes = dto.ExpectedMinutes;
            task.DueDate = dto.DueDate;
            task.Status = dto.Status;
            task.Note = dto.Note;
            task.FailureReasonId = dto.FailureReasonId;

            await _context.SaveChangesAsync();
            return true;
        }

        // 6. حذف مهمة (حذف ناعم)
        public async Task<bool> DeleteTaskAsync(int id, string anonymousId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.Week.AnonymousId == anonymousId && !t.IsDeleted);

            if (task == null)
                return false;

            task.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // 7. **إنهاء المهمة بنجاح** (شاشة "أحسنت!")
        public async Task<bool> CompleteTaskAsync(int id, CompleteTaskDto dto, string anonymousId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.Week.AnonymousId == anonymousId && !t.IsDeleted);

            if (task == null)
                return false;

            // التأكد من أن المهمة لم تُنجز سابقاً
            if (task.Status == TaskItemStatus.Completed)
                throw new InvalidOperationException("هذه المهمة مكتملة بالفعل.");

            // تحديث الحالة إلى "منجزة"
            task.Status = TaskItemStatus.Completed;
            task.ActualMinutes = dto.ActualMinutes; // تخزين الوقت الفعلي (الكنز لتحليل التسويف)
            task.Note = dto.Note ?? task.Note; // تحديث الملاحظة إذا أرسلها المستخدم
            task.ImageUrl = dto.ImageUrl ?? task.ImageUrl; // تحديث الصورة إذا رفعها

            await _context.SaveChangesAsync();
            return true;
        }

        // 8. **فشل المهمة** (شاشة "سبب عدم الإنجاز")
        public async Task<bool> FailTaskAsync(int id, int failureReasonId, string? note, string anonymousId)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.Week.AnonymousId == anonymousId && !t.IsDeleted);

            if (task == null)
                return false;

            // التحقق من وجود سبب الفشل في قاعدة البيانات
            var failureReason = await _context.FailureReasons
                .FirstOrDefaultAsync(fr => fr.Id == failureReasonId);

            if (failureReason == null)
                throw new KeyNotFoundException("سبب الفشل غير موجود.");

            // تحديث الحالة إلى "فاشلة"
            task.Status = TaskItemStatus.Failed;
            task.FailureReasonId = failureReasonId;
            task.Note = note ?? task.Note;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}