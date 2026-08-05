namespace TaskManagement.Enums
{
    // التعداد (Enum) يحدد درجة أهمية المهمة
    // (byte) يعني أنه سيُخزن في قاعدة البيانات كرقم صغير لتوفير المساحة
    public enum PriorityLevel : byte
    {
        Low = 1,      // أهمية منخفضة
        Medium = 2,   // أهمية متوسطة
        High = 3      // أهمية عالية (الأولوية القصوى)
    }
}