using System;

namespace CSharpCornerApi.Models
{
    public class UserHistory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
