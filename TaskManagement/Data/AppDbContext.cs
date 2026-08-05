using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    // هذه الفئة ترث من DbContext، وهي تمثل "جلسة العمل" مع قاعدة البيانات
    // مسؤوليتها: ربط الكلاسات (Models) بالجداول الفعلية في SQL Server
    public class AppDbContext : DbContext
    {
        // المُنشئ (Constructor): يستقبل خيارات الاتصال بقاعدة البيانات ويمررها للفئة الأم
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // هذه الخصائص تمثل الجداول (Tables) في قاعدة البيانات
        // عندما نضيف أو نستعلم بيانات، نتعامل مع هذه الـ DbSets
        public DbSet<Week> Weeks { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<FailureReason> FailureReasons { get; set; }

        // هذه الدالة تُستخدم لتخصيص إعدادات الجداول والعلاقات (Fluent API)
        // يتم تشغيلها مرة واحدة عند إنشاء النموذج (Model) لأول مرة
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- العلاقة الأولى: الأسبوع (Week) مع المهام (TaskItem) ---
            // الأسبوع الواحد (One) يحتوي على عدة مهام (Many)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Week)                // المهمة لها أسبوع واحد
                .WithMany(w => w.Tasks)             // الأسبوع يحتوي على عدة مهام
                .HasForeignKey(t => t.WeekId)       // المفتاح الخارجي هو WeekId في جدول المهام
                .OnDelete(DeleteBehavior.Cascade);  // إذا حُذف الأسبوع، تُحذف جميع مهامه تلقائياً (لأنها لا معنى لها بدونه)

            // --- العلاقة الثانية: المهمة (TaskItem) مع سبب الفشل (FailureReason) ---
            // المهمة قد يكون لها سبب فشل واحد، أو لا شيء (Many-to-One)
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.FailureReason)        // المهمة قد يكون لها سبب فشل واحد
                .WithMany(f => f.Tasks)              // سبب الفشل الواحد قد يتكرر في عدة مهام
                .HasForeignKey(t => t.FailureReasonId) // المفتاح الخارجي هو FailureReasonId
                .OnDelete(DeleteBehavior.SetNull);   // إذا حُذف سبب الفشل من الجدول، نضع المفتاح الخارجي في المهام = NULL (حتى لا نفقد المهام)

            // --- إعداد إضافي لتحديد دقة التواريخ في SQL Server (اختياري لكن مفيد) ---
            // نمنع SQL Server من تخزين أجزاء الملي ثانية لتوفير المساحة
            modelBuilder.Entity<Week>()
                .Property(w => w.StartDate)
                .HasColumnType("datetime2(0)"); // دقة حتى الثانية فقط

            modelBuilder.Entity<Week>()
                .Property(w => w.EndDate)
                .HasColumnType("datetime2(0)");

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.CreatedAt)
                .HasColumnType("datetime2(0)");
        }
    }
}