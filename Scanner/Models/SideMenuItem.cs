
namespace Scanner.Models
{
    public class SideMenuItem
    {
        public string MenuName { get; set; }
        public string MenuId { get; set; }
        public string OnlyFor { get; set; }
        public bool IsSubMenu { get; set; }
    }
}