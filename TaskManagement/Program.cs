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

// --- 2. إضافة الخدمات (Services) إلى حاوية Dependency Injection ---

// 2.1 تسجيل DbContext باستخدام SQLite (بدلاً من SQL Server)
// سلسلة الاتصال موجودة في appsettings.json تحت "DefaultConnection"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2.2 إضافة الـ Controllers
builder.Services.AddControllers();

// 2.3 تسجيل معالج الأخطاء العالمي
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// 2.4 تسجيل الخدمات (Dependency Injection)
builder.Services.AddScoped<IWeekService, WeekService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// 2.5 إضافة خدمة Swagger (توثيق الـ API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2.6 تفعيل FluentValidation للتحقق من صحة البيانات
builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// --- 3. بناء التطبيق ---
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

// 4.3 تفعيل نظام المصادقة (تحسباً للمستقبل)
app.UseAuthorization();

// 4.4 تفعيل خدمة الملفات الثابتة (لخدمة ملفات HTML, CSS, JS من مجلد wwwroot)
app.UseStaticFiles();

// 4.5 تفعيل معالج الأخطاء العالمي (يجب أن يكون قبل MapControllers)
app.UseExceptionHandler();

// 4.6 تحديد مسارات الـ Controllers
app.MapControllers();

// --- 5. إعداد قاعدة البيانات والبيانات الأولية (Seed Data) ---
using (var scope = app.Services.CreateScope())
{
    // الحصول على نسخة من DbContext
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // ✅ التأكد من وجود قاعدة البيانات والجداول (لـ SQLite)
    // هذا الأمر ينشئ الملف (.db) والجداول إذا لم تكن موجودة مسبقاً
    dbContext.Database.EnsureCreated();

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

// --- 6. تشغيل التطبيق ---
app.Run();