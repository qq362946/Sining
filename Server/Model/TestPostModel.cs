using SqlSugar;

namespace Sining.Model
{
    public class TestPostModel : Component
    {
        [SugarColumn(Length = 21)]
        public string Name { get; set; }
        [SugarColumn(Length = 21)]
        public string PassWord{ get; set; }
    }
}