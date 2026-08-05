using TaskManagement.Enums;

namespace TaskManagement.Dtos
{
    // هذا الكائن يُستخدم عند عرض تفاصيل مهمة واحدة
    public class TaskReadDto
    {
        // المعرف الفريد للمهمة
        public int Id { get; set; }

        // عنوان المهمة
        public string Title { get; set; } = string.Empty;

        // وصف المهمة
        public string? Description { get; set; }

        // الأولوية (نرسلها كنص لتظهر مباشرة في الشاشة دون تحويل)
        public string Priority { get; set; } = string.Empty;

        // الصعوبة (نرسلها كنص)
        public string Difficulty { get; set; } = string.Empty;

        // الوقت المتوقع بالدقائق
        public int ExpectedMinutes { get; set; }

        // الوقت الفعلي المستغرق (إن وجد)
        public int? ActualMinutes { get; set; }

        // تاريخ الاستحقاق
        public DateTime? DueDate { get; set; }

        // حالة المهمة (نرسلها كنص لتظهر كـ "قيد التنفيذ" أو "تمت" أو "فشلت")
        public string Status { get; set; } = string.Empty;

        // ملاحظة المستخدم
        public string? Note { get; set; }

        // رابط الصورة المرفقة
        public string? ImageUrl { get; set; }

        // تاريخ بداية الأسبوع الذي تنتمي إليه المهمة (للسياق)
        public DateTime WeekStartDate { get; set; }

        // نص سبب الفشل (إذا كانت الحالة = Failed)
        public string? FailureReason { get; set; }

        // تاريخ إنشاء المهمة
        public DateTime CreatedAt { get; set; }
    }
}