using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagement.Enums;

namespace TaskManagement.Models
{
    // هذا الكيان يمثل جدول "المهام" في قاعدة البيانات (الجدول الأكثر أهمية)
    public class TaskItem
    {
        // المفتاح الأساسي (Primary Key)
        public int Id { get; set; }

        // عنوان المهمة (مثال: "إنهاء API المشروع")
        // [Required] => إلزامي
        // [MaxLength(300)] => الحد الأقصى للأحرف 300
        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        // وصف المهمة (اختياري، يمكن أن يكون فارغاً)
        public string? Description { get; set; }

        // الأولوية (عالية/متوسطة/منخفضة) - يتم تخزينها كرقم (1,2,3) من الـ Enum
        public PriorityLevel Priority { get; set; }

        // الصعوبة (صعبة/متوسطة/سهلة) - يتم تخزينها كرقم (1,2,3) من الـ Enum
        public DifficultyLevel Difficulty { get; set; }

        // الوقت المتوقع بالدقائق (مثال: 120 دقيقة)
        // [Required] => إلزامي
        [Required]
        public int ExpectedMinutes { get; set; }

        // الوقت الفعلي المستغرق بالدقائق (اختياري).
        // هذه الخاصية هي "الكنز" في نظام تحليل التسويف، لأننا نقارنها مع ExpectedMinutes.
        public int? ActualMinutes { get; set; }

        // التاريخ المحدد لإنجاز المهمة (اختياري)
        public DateTime? DueDate { get; set; }

        // حالة المهمة (Waiting / Completed / Failed) - يتم تخزينها كرقم (0,1,2)
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Waiting;

        // ملاحظة إضافية من المستخدم (اختياري)
        public string? Note { get; set; }

        // رابط صورة مرفقة (اختياري) - سنخزن مسار الملف هنا
        public string? ImageUrl { get; set; }

        // تاريخ إنشاء المهمة (يُحدد تلقائياً)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // خاصية الحذف الناعم (Soft Delete) للمهمة
        public bool IsDeleted { get; set; } = false;

        // ---------- المفاتيح الخارجية (Foreign Keys) ----------
        // المفتاح الخارجي الذي يربط المهمة بجدول الأسابيع
        public int WeekId { get; set; }

        // المفتاح الخارجي الذي يربط المهمة بجدول أسباب الفشل (اختياري، أي يمكن أن يكون فارغاً)
        public int? FailureReasonId { get; set; }

        // ---------- علاقات التنقل (Navigation Properties) ----------
        // كل مهمة تتبع لأسبوع واحد فقط
        // [ForeignKey] يحدد بوضوح أي عمود هو المفتاح الخارجي
        [ForeignKey(nameof(WeekId))]
        public virtual Week Week { get; set; } = null!;

        // كل مهمة يمكن أن يكون لها سبب فشل واحد (أو لا شيء)
        [ForeignKey(nameof(FailureReasonId))]
        public virtual FailureReason? FailureReason { get; set; }
    }
}