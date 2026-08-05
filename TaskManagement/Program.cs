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

// --- 2. إضافة الخدمات ---

// 2.1 تسجيل DbContext باستخدام SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2.2 إضافة الـ Controllers
builder.Services.AddControllers();

// 2.3 تسجيل معالج الأخطاء العالمي (هام جداً)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // هذا يضمن توليد ProblemDetails

// 2.4 تسجيل الخدمات
builder.Services.AddScoped<IWeekService, WeekService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// 2.5 إضافة Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2.6 تفعيل FluentValidation
builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// --- 3. بناء التطبيق ---
var app = builder.Build();

// --- 4. إعداد خط أنابيب معالجة الطلبات ---

// 4.1 تشغيل Swagger في بيئة التطوير
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4.2 إعادة توجيه HTTPS
app.UseHttpsRedirection();

// 4.3 تفعيل المصادقة
app.UseAuthorization();

// 4.4 تفعيل الملفات الثابتة (wwwroot)
app.UseStaticFiles();

// 4.5 تفعيل معالج الأخطاء العالمي (سطر واحد فقط، بدون معاملات)
app.UseExceptionHandler();

// 4.6 تحديد مسارات الـ Controllers
app.MapControllers();

// --- 5. إعداد قاعدة البيانات والبيانات الأولية ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // إنشاء قاعدة البيانات والجداول (لـ SQLite)
    dbContext.Database.EnsureCreated();

    // إضافة أسباب الفشل إذا لم تكن موجودة
    if (!dbContext.FailureReasons.Any())
    {
        dbContext.FailureReasons.AddRange(
            new FailureReason { Reason = "نسيت" },
            new FailureReason { Reason = "لم يكن لدي وقت" },
            new FailureReason { Reason = "كانت أصعب مما توقعت" },
            new FailureReason { Reason = "فقدت الحماس" },
            new FailureReason { Reason = "سبب آخر" }
        );
        dbContext.SaveChanges();
        Console.WriteLine("✅ تم إضافة أسباب الفشل الخمسة إلى قاعدة البيانات بنجاح.");
    }
}

// --- 6. تشغيل التطبيق ---
app.Run();