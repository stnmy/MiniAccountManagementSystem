namespace MiniAccountManagementSystem.Dtos
{
    public class AccountTreeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public string HierarchyPath { get; set; }
    }
}
