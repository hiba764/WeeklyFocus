using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Dtos
{
    // هذا الكائن يُستخدم عندما يريد المستخدم إنشاء أسبوع جديد
    // يحدد شكل البيانات القادمة من الطلب (Request Body)
    public class WeekCreateDto
    {
        // تاريخ بداية الأسبوع (مثل 2026-08-01)
        // [Required] يعني أن المستخدم ملزم بإرساله، وإلا سيرفض النظام الطلب
        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        public DateTime StartDate { get; set; }

        // تاريخ نهاية الأسبوع (مثل 2026-08-07)
        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        public DateTime EndDate { get; set; }

        // ملاحظة: لا يوجد هنا GoalPercentage (نسبة الإنجاز) لأننا سنحسبها آلياً
        // ولا يوجد AnonymousId لأننا سنولده في الخدمة تلقائياً
    }
}