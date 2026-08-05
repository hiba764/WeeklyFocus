using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Dtos
{
    // هذا الكائن يُستخدم عند فشل المهمة (شاشة "لم يتم إنجاز المهمة")
    public class FailTaskDto
    {
        // معرف سبب الفشل (مثل 1 = نسيت، 2 = لم يكن لدي وقت ... إلخ)
        [Required(ErrorMessage = "سبب الفشل مطلوب")]
        public int FailureReasonId { get; set; }

        // ملاحظة اختيارية (مثل "كنت مشغولاً جداً")
        public string? Note { get; set; }
    }
}