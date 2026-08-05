namespace TaskManagement.Enums
{
    // التعداد (Enum) يحدد أين توجد المهمة في دورة حياتها
    public enum TaskItemStatus : byte
    {
        Waiting = 0,   // في الانتظار (لم تبدأ بعد)
        Completed = 1, // تمت بنجاح
        Failed = 2     // فشلت أو لم تكتمل في الموعد
    }
}