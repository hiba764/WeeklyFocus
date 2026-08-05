using TaskManagement.Dtos;

namespace TaskManagement.Interfaces
{
    // هذا العقد (Contract) يحدد جميع الدوال التي يجب أن يوفرها TaskService
    public interface ITaskService
    {
        // جلب جميع المهام الخاصة بأسبوع معين (تظهر في الصفحة الرئيسية)
        // إرجاع: قائمة من TaskReadDto
        Task<IEnumerable<TaskReadDto>> GetTasksByWeekAsync(int weekId, string anonymousId);

        // **خاصية مهام اليوم**: جلب المهام التي حالتها Waiting وتاريخ استحقاقها (DueDate) يساوي تاريخ اليوم
        // تستخدم في الصفحة الرئيسية (Dashboard) تحت عنوان "مهام اليوم"
        Task<IEnumerable<TaskReadDto>> GetTodayTasksAsync(int weekId, string anonymousId);

        // جلب مهمة محددة بواسطة Id لعرض تفاصيلها (الشاشة السادسة)
        Task<TaskReadDto> GetTaskByIdAsync(int id, string anonymousId);

        // إنشاء مهمة جديدة (شاشة إضافة مهمة)
        // إرجاع: TaskReadDto للمهمة التي تم إنشاؤها
        Task<TaskReadDto> CreateTaskAsync(TaskCreateDto dto, string anonymousId);

        // تحديث مهمة موجودة (جميع الحقول عدا WeekId)
        // إرجاع: true إذا تم التحديث بنجاح
        Task<bool> UpdateTaskAsync(int id, TaskUpdateDto dto, string anonymousId);

        // حذف مهمة (حذف ناعم)
        Task<bool> DeleteTaskAsync(int id, string anonymousId);

        // **عملية إنهاء المهمة**: تغيير الحالة إلى Completed، وتحديد الوقت الفعلي (ActualMinutes) والملاحظات
        // هذه العملية تستجيب لشاشة "أحسنت!" (الشاشة الثالثة)
        Task<bool> CompleteTaskAsync(int id, CompleteTaskDto dto, string anonymousId);

        // **عملية فشل المهمة**: تغيير الحالة إلى Failed، وربط سبب الفشل (FailureReasonId) مع ملاحظة
        // هذه العملية تستجيب لشاشة "سبب عدم الإنجاز" (الشاشة الرابعة)
        Task<bool> FailTaskAsync(int id, int failureReasonId, string? note, string anonymousId);
    }
}