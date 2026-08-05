using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Dtos
{
    // هذا الكائن يُستخدم عند إضافة مهمة جديدة (شاشة إضافة مهمة)
    public class TaskCreateDto
    {
        // عنوان المهمة (مثال: "إنهاء API المشروع")
        [Required(ErrorMessage = "عنوان المهمة مطلوب")]
        [MaxLength(300, ErrorMessage = "العنوان لا يتجاوز 300 حرف")]
        public string Title { get; set; } = string.Empty;

        // وصف المهمة (اختياري)
        public string? Description { get; set; }

        // الأولوية: 1 = منخفضة، 2 = متوسطة، 3 = عالية
        [Required(ErrorMessage = "الأولوية مطلوبة")]
        [Range(1, 3, ErrorMessage = "الأولوية يجب أن تكون 1 أو 2 أو 3")]
        public byte Priority { get; set; }

        // الصعوبة: 1 = سهلة، 2 = متوسطة، 3 = صعبة
        [Required(ErrorMessage = "الصعوبة مطلوبة")]
        [Range(1, 3, ErrorMessage = "الصعوبة يجب أن تكون 1 أو 2 أو 3")]
        public byte Difficulty { get; set; }

        // الوقت المتوقع بالدقائق (مثال: 120 دقيقة)
        [Required(ErrorMessage = "الوقت المتوقع مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "الوقت المتوقع يجب أن يكون أكبر من 0")]
        public int ExpectedMinutes { get; set; }

        // تاريخ استحقاق المهمة (اختياري)
        public DateTime? DueDate { get; set; }

        // معرف الأسبوع الذي تنتمي إليه هذه المهمة
        [Required(ErrorMessage = "معرف الأسبوع مطلوب")]
        public int WeekId { get; set; }
    }
}