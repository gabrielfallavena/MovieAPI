using System.Text.Json.Serialization;

namespace API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public float Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string MovieTitle { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int MovieId { get; set; }

        // Relacionamento de muitos-para-um: muitas reviews podem pertencer a um único usuário
        public int UserId { get; set; }  // Chave estrangeira para o usuário
        [JsonIgnore]
        public User User { get; set; }  // Referência ao objeto User
    }
}
