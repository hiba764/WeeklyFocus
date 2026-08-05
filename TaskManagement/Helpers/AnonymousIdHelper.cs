namespace TaskManagement.Helpers
{
    // هذه الفئة تحتوي على دوال مساعدة عامة تستخدم في جميع أنحاء المشروع
    public static class AnonymousIdHelper
    {
        // دالة مساعدة لتوليد معرف مستخدم تلقائياً إذا لم يرسله العميل
        // هذه الدالة (Static) يمكن استدعاؤها من أي مكان دون الحاجة لإنشاء كائن منها
        public static string GetOrGenerate(string? providedId)
        {
            // إذا كانت القيمة المرسلة فارغة أو null، نولد قيمة جديدة فريدة (GUID)
            if (string.IsNullOrWhiteSpace(providedId))
                return Guid.NewGuid().ToString();

            // إذا كانت القيمة موجودة، نستخدمها كما هي
            return providedId;
        }
    }
}