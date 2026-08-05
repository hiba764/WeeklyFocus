using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Dtos
{
    // هذا الكائن يُستخدم عند إنهاء مهمة (شاشة "أحسنت!")
    public class CompleteTaskDto
    {
        // الوقت الفعلي الذي استغرقه المستخدم لإنجاز المهمة (بالدقائق)
        // هذا الحقل هو "الكنز" الذي سنستخدمه لتحليل التسويف لاحقاً
        [Required(ErrorMessage = "الوقت الفعلي مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "الوقت الفعلي يجب أن يكون أكبر من 0")]
        public int ActualMinutes { get; set; }

        // ملاحظة اختيارية يكتبها المستخدم عن تجربة الإنجاز
        public string? Note { get; set; }

        // رابط الصورة المرفقة (مثل صورة إثبات الإنجاز)
        public string? ImageUrl { get; set; }
    }
}