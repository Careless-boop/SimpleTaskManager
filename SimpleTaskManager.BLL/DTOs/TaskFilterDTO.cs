using SimpleTaskManager.DAL.Enums;

namespace SimpleTaskManager.BLL.DTOs
{
    public class TaskFilterDTO
    {
        //filters
        public string? Title { get; set; }
        public Status? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public Priority? Priority { get; set; }

        //sorting
        public TaskSortBy? SortBy { get; set; }
        public bool SortDescending { get; set; }

        //pagintaion
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
