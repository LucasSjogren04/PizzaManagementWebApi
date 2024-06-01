using Microsoft.AspNetCore.Identity;

namespace Tomaso_Pizza.Data.Entities
{
    public class Points
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public int PointsCount { get; set; }
    }
}
