using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Middleware
{
    // هذه الفئة هي المعالج المركزي لجميع الأخطاء في التطبيق
    // ترث من IExceptionHandler (الواجهة الرسمية في .NET لمعالجة الأخطاء)
    public class GlobalExceptionHandler : IExceptionHandler
    {
        // متغير خاص لتسجيل الأخطاء (Logging) لمساعدتنا في تتبع المشاكل أثناء التشغيل
        private readonly ILogger<GlobalExceptionHandler> _logger;

        // المُنشئ: يحقن (inject) خدمة التسجيل (Logger) من النظام
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        // هذه هي الدالة الرئيسية التي يتم استدعاؤها تلقائياً عند حدوث أي استثناء
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,                // معلومات الطلب الحالي
            Exception exception,                    // الاستثناء الذي حدث
            CancellationToken cancellationToken)    // إشارة لإلغاء العملية (نادراً ما تستخدم)
        {
            // --- 1. تسجيل الخطأ في سجل التطبيق (Console / ملفات السجل) ---
            // هذا يساعد المطور في معرفة تفاصيل الخطأ دون الحاجة لتصحيح الأخطاء (Debugging)
            _logger.LogError(exception, "حدث خطأ غير متوقع: {Message}", exception.Message);

            // --- 2. تحديد رمز حالة HTTP المناسب بناءً على نوع الخطأ ---
            // سنميز بين خطأ "العنصر غير موجود" (404) وباقي الأخطاء (500)
            var statusCode = exception switch
            {
                // إذا كان الخطأ من نوع KeyNotFoundException (الذي نرميه عندما لا نجد أسبوعاً أو مهمة)
                KeyNotFoundException => StatusCodes.Status404NotFound,

                // إذا كان الخطأ من نوع ArgumentException (الذي نرميه عندما تكون التواريخ غير صحيحة)
                ArgumentException => StatusCodes.Status400BadRequest,

                // إذا كان الخطأ من نوع InvalidOperationException (مثل محاولة إنهاء مهمة مكتملة سابقاً)
                InvalidOperationException => StatusCodes.Status400BadRequest,

                // أي خطأ آخر غير متوقع (مشكلة في قاعدة البيانات أو الكود) نعتبره خطأ خادم داخلي
                _ => StatusCodes.Status500InternalServerError
            };

            // --- 3. إنشاء كائن الرد الموحد (ProblemDetails) الذي سيراه المستخدم ---
            // هذا الكائن يتبع معيار RFC 7807 (وهو معيار عالمي لوصف الأخطاء في APIs)
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "حدث خطأ أثناء معالجة طلبك",
                Detail = exception.Message, // نرسل رسالة الخطأ التفصيلية للمطور (في بيئة التطوير)
                Instance = httpContext.Request.Path, // نرسل المسار الذي حدث فيه الخطأ
                Type = exception.GetType().Name // نرسل نوع الخطأ (مثل KeyNotFoundException)
            };

            // --- 4. إرسال الرد إلى العميل (المستخدم) بصيغة JSON ---
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            // كتابة الرد في تدفق الاستجابة (Response Stream)
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            // إرجاع true: يعني أننا تعاملنا مع هذا الخطأ بنجاح، ولا داعي لمعالجات أخرى
            return true;
        }
    }
}