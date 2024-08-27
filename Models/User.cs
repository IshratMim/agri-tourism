using System.Collections.Generic;

namespace ReviewSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        // Navigation property for one-to-many relationship
        public List<Review> Reviews { get; set; }
    }
}

