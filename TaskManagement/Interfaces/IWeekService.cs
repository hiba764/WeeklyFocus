using TaskManagement.Dtos;

namespace TaskManagement.Interfaces
{
    // هذا العقد (Contract) يحدد جميع الدوال التي يجب أن يوفرها WeekService
    // كل دوالنا ستكون غير متزامنة (Async) لتجنب حظر الخادم عند التعامل مع قاعدة البيانات
    public interface IWeekService
    {
        // جلب جميع الأسابيع الخاصة بمستخدم معين (عن طريق AnonymousId)
        // إرجاع: قائمة من WeekReadDto
        Task<IEnumerable<WeekReadDto>> GetAllWeeksAsync(string anonymousId);

        // جلب أسبوع محدد بواسطة Id، والتأكد من أنه يخص المستخدم الحالي
        // إرجاع: WeekReadDto مع جميع تفاصيله وإحصائياته
        Task<WeekReadDto> GetWeekByIdAsync(int id, string anonymousId);

        // إنشاء أسبوع جديد (جلب البيانات من WeekCreateDto)
        // إرجاع: WeekReadDto للأسبوع الذي تم إنشاؤه (مع Id الجديد ونسبة الإنجاز)
        Task<WeekReadDto> CreateWeekAsync(WeekCreateDto dto, string anonymousId);

        // تحديث أسبوع موجود (التواريخ فقط)
        // إرجاع: true إذا تم التحديث بنجاح، false إذا لم يتم العثور على الأسبوع
        Task<bool> UpdateWeekAsync(int id, WeekCreateDto dto, string anonymousId);

        // حذف أسبوع (حذف ناعم Soft Delete، أي وضع علامة IsDeleted = true)
        // إرجاع: true إذا تم الحذف بنجاح
        Task<bool> DeleteWeekAsync(int id, string anonymousId);

        // **الميزة الذكية**: جلب جميع إحصائيات "مرآة الأسبوع" (التي تظهر في الشاشة الخامسة)
        // تشمل: عدد المهام، المنجزة، النسبة، النقاط، أكثر سبب للتأجيل، أصعب مهمة، ورسالة تحفيزية
        Task<WeekInsightsDto> GetWeekInsightsAsync(int id, string anonymousId);
    }
}