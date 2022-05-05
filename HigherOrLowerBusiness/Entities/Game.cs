using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Entities
{
    [Table("games")]
    class Game
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("cards")]
        [Required]
        public string Cards { get; set; }

        [Column("current_round")]
        [Required]
        public int CurrentRound { get; set; }

        public Game(string cards)
        {
            this.Cards = cards;
            this.CurrentRound = 0;

        }

    }
}
