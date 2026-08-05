using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    // هذا الكيان يمثل جدول "أسباب الفشل" في قاعدة البيانات
    public class FailureReason
    {
        // المفتاح الأساسي (Primary Key) - يتزايد تلقائياً
        public int Id { get; set; }

        // النص التوضيحي للسبب (مثال: "لم يكن لدي وقت")
        // [Required] يعني أنه حقل إلزامي لا يمكن أن يكون فارغاً
        // [MaxLength(200)] يعني أن طول النص لا يتجاوز 200 حرف لتوفير مساحة التخزين
        [Required, MaxLength(200)]
        public string Reason { get; set; } = string.Empty;

        // خاصية التنقل (Navigation Property): سبب الفشل الواحد يمكن أن يرتبط بالعديد من المهام
        // هذا يمثل العلاقة (One-to-Many) مع جدول المهام
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}