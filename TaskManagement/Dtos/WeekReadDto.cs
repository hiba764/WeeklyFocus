namespace TaskManagement.Dtos
{
    // هذا الكائن يُستخدم عندما يطلب المستخدم عرض بيانات أسبوع معين
    // يحدد شكل البيانات القادمة في الرد (Response)
    public class WeekReadDto
    {
        // المعرف الفريد للأسبوع (يُستخدم في الروابط للتعديل أو الحذف)
        public int Id { get; set; }

        // تاريخ بداية الأسبوع
        public DateTime StartDate { get; set; }

        // تاريخ نهاية الأسبوع
        public DateTime EndDate { get; set; }

        // **نسبة الإنجاز**: هذه القيمة تُحسب في الـ Service وليست مخزنة في قاعدة البيانات
        // مثلاً: (عدد المهام المنجزة / عدد المهام الكلي) * 100
        public double GoalPercentage { get; set; }

        // عدد المهام الكلي في هذا الأسبوع (يُحسب في الـ Service)
        public int TasksCount { get; set; }

        // عدد المهام المنجزة في هذا الأسبوع (يُحسب في الـ Service)
        public int CompletedTasks { get; set; }

        // **نقاط الخبرة (XP)**: مجموع النقاط المكتسبة هذا الأسبوع
        // مثلاً: مجموع (الوقت المتوقع * معامل الصعوبة) للمهام المنجزة
        public int TotalXp { get; set; }

        // تاريخ إنشاء الأسبوع في النظام (يُستخدم للترتيب)
        public DateTime CreatedAt { get; set; }
    }
}