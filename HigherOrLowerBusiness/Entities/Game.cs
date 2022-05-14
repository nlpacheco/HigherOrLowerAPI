using HigherOrLowerData.Business;
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
    public class Game
    {
        [Key]
        [Column("id")]
        public uint Id { get; set; }


        [Column("cards")]
        //[MaxLength(500)]
        [Required]
        public string Cards { get; set; }



        [Column("player_count")]
        [Required]
        public int PlayerCount { get; set; }


        [Column("current_round")]
        [Required]
        public int CurrentRound { get; set; }

        public IEnumerable<GamePlayer>? Players { get; set; }


        public Game()
        {
            this.Cards = "";
            this.PlayerCount = 0;
            this.CurrentRound = 0;
        }


       public Game(string cards, int countOfPlayers)
        {
            this.Cards = cards;
            this.PlayerCount = countOfPlayers;
            this.CurrentRound = 0;
        }


        public Game(DeckOfCards cards, int countOfPlayers)
        {
            //this.DeckOfCards = cards;
            this.Cards = cards.getCardsAsText();
            this.PlayerCount = countOfPlayers;
            this.CurrentRound = 0;
        }


    }
}
