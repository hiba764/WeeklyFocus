using System.ComponentModel.DataAnnotations;
using TaskManagement.Enums;

namespace TaskManagement.Dtos
{
    // هذا الكائن يُستخدم عند تعديل مهمة موجودة
    public class TaskUpdateDto
    {
        // عنوان المهمة
        [Required(ErrorMessage = "عنوان المهمة مطلوب")]
        [MaxLength(300, ErrorMessage = "العنوان لا يتجاوز 300 حرف")]
        public string Title { get; set; } = string.Empty;

        // وصف المهمة (اختياري)
        public string? Description { get; set; }

        // الأولوية
        [Required(ErrorMessage = "الأولوية مطلوبة")]
        [Range(1, 3, ErrorMessage = "الأولوية يجب أن تكون 1 أو 2 أو 3")]
        public byte Priority { get; set; }

        // الصعوبة
        [Required(ErrorMessage = "الصعوبة مطلوبة")]
        [Range(1, 3, ErrorMessage = "الصعوبة يجب أن تكون 1 أو 2 أو 3")]
        public byte Difficulty { get; set; }

        // الوقت المتوقع
        [Required(ErrorMessage = "الوقت المتوقع مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "الوقت المتوقع يجب أن يكون أكبر من 0")]
        public int ExpectedMinutes { get; set; }

        // تاريخ الاستحقاق
        public DateTime? DueDate { get; set; }

        // حالة المهمة (0 = في الانتظار، 1 = منجزة، 2 = فاشلة)
        public TaskItemStatus Status { get; set; }

        // ملاحظة إضافية
        public string? Note { get; set; }

        // معرف سبب الفشل (إذا كانت الحالة = Failed)
        public int? FailureReasonId { get; set; }
    }
}