using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskManagement.Data;
using TaskManagement.Interfaces;
using TaskManagement.Middleware;
using TaskManagement.Models;
using TaskManagement.Services;

// --- 1. بناء التطبيق ---
var builder = WebApplication.CreateBuilder(args);
//
builder.Services.AddProblemDetails();

// --- 2. إضافة الخدمات (Services) إلى حاوية Dependency Injection ---

// 2.1 تسجيل DbContext الخاص بقاعدة البيانات (ربط السيرفر)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2.2 إضافة الـ Controllers (جعل الـ API يعمل)
builder.Services.AddControllers();
// تسجيل معالج الأخطاء العالمي
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// تسجيل الخدمات (Services) في حاوية Dependency Injection
builder.Services.AddScoped<IWeekService, WeekService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// 2.3 إضافة خدمة Swagger (صفحة توثيق الـ API التي تختبر منها)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2.4 تفعيل FluentValidation للتحقق من صحة البيانات القادمة من المستخدم
builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// --- 3. بناء التطبيق (تنفيذ البناء) ---
var app = builder.Build();

// --- 4. إعداد خط أنابيب معالجة الطلبات (Middleware) ---

// 4.1 تشغيل صفحة Swagger فقط في بيئة التطوير (وهذا ما أنت فيه)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4.2 إعادة توجيه طلبات HTTP إلى HTTPS
app.UseHttpsRedirection();

// 4.3 تفعيل نظام المصادقة (حتى لو لم نفعله الآن، نضعه تحسباً للمستقبل)
app.UseAuthorization();

// تفعيل خدمة الملفات الثابتة (لكي نتمكن من عرض ملفات HTML و CSS و JS من مجلد wwwroot)
app.UseStaticFiles();

// تفعيل معالج الأخطاء العالمي (يجب أن يأتي قبل MapControllers)
app.UseExceptionHandler();
// 4.4 تحديد مسارات الـ Controllers
app.MapControllers();

// --- 5. إضافة البيانات الأولية (Seed Data) لقاعدة البيانات ---
// تتأكد من وجود أسباب الفشل الأساسية في قاعدة البيانات منذ البداية
using (var scope = app.Services.CreateScope())
{
    // الحصول على نسخة من DbContext للتعامل مع قاعدة البيانات
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // التحقق: هل جدول أسباب الفشل فارغ؟ (أي لا يحتوي على أي بيانات)
    if (!dbContext.FailureReasons.Any())
    {
        // إضافة الأسباب الخمسة الرئيسية التي تظهر في شاشة المستخدم
        dbContext.FailureReasons.AddRange(
            new FailureReason { Reason = "نسيت" },
            new FailureReason { Reason = "لم يكن لدي وقت" },
            new FailureReason { Reason = "كانت أصعب مما توقعت" },
            new FailureReason { Reason = "فقدت الحماس" },
            new FailureReason { Reason = "سبب آخر" }
        );

        // حفظ التغييرات في قاعدة البيانات (إدراج السجلات)
        dbContext.SaveChanges();

        // طباعة رسالة نجاح في نافذة الإخراج (Console)
        Console.WriteLine("✅ تم إضافة أسباب الفشل الخمسة إلى قاعدة البيانات بنجاح.");
    }
}


app.UseExceptionHandler();

// --- 6. تشغيل التطبيق ---
app.Run();