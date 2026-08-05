namespace TaskManagement.Dtos
{
    // هذا الكائن يُستخدم لعرض إحصائيات "مرآة الأسبوع" المتقدمة
    // يطابق الشاشة الخامسة في تصميم الواجهة
    public class WeekInsightsDto
    {
        // --- الإحصائيات الأساسية ---
        public int TotalTasks { get; set; }        // عدد المهام الكلي في الأسبوع
        public int CompletedTasks { get; set; }    // عدد المهام المنجزة
        public double CompletionPercentage { get; set; } // نسبة الإنجاز (مثل 75.0)

        // --- نظام النقاط (XP) ---
        public int TotalXp { get; set; }           // مجموع نقاط الخبرة (مثل 240 XP)

        // --- تحليل التسويف (الجزء الذكي) ---
        public string? TopFailureReason { get; set; } // أكثر سبب تكرر في فشل المهام (مثل "لم يكن لدي وقت")
        public int TopFailureReasonCount { get; set; } // كم مرة تكرر هذا السبب

        public string? HardestCompletedTask { get; set; } // عنوان أصعب مهمة تم إنجازها (حسب Difficulty = Hard)

        // --- الرسالة التحفيزية (المستقبلية) ---
        // هذه الرسالة تُبنى بناءً على النسبة المئوية وعدد المهام الفاشلة
        public string MotivationalMessage { get; set; } = string.Empty;
    }
}