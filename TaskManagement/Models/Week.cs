using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    // هذا الكيان يمثل جدول "الأسابيع" في قاعدة البيانات
    public class Week
    {
        // المفتاح الأساسي (Primary Key)
        public int Id { get; set; }

        // هذا العمود يمثل هوية المستخدم المؤقتة (دون الحاجة لتسجيل دخول).
        // عند إنشاء أسبوع جديد، نمنحه قيمة عشوائية (GUID) لتمييزه عن أسابيع المستخدمين الآخرين.
        public string AnonymousId { get; set; } = Guid.NewGuid().ToString();

        // تاريخ بداية الأسبوع (مثل 2026-08-01)
        // [Required] يعني أن المستخدم ملزم بإدخاله
        [Required]
        public DateTime StartDate { get; set; }

        // تاريخ نهاية الأسبوع (مثل 2026-08-07)
        [Required]
        public DateTime EndDate { get; set; }

        // تاريخ إنشاء السجل في النظام (يُحدد تلقائياً وقت الإضافة)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // خاصية الحذف الناعم (Soft Delete).
        // إذا كانت قيمتها true، يعني أن المستخدم حذف الأسبوع ولكننا نحتفظ بالبيانات في قاعدة البيانات لأغراض التحليل المستقبلي.
        public bool IsDeleted { get; set; } = false;

        // خاصية التنقل (Navigation Property): الأسبوع الواحد يحتوي على قائمة من المهام
        // هذا يمثل العلاقة (One-to-Many) مع جدول المهام
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}