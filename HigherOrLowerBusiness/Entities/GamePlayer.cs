using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Entities
{
    [Table("game_players")]
    internal class GamePlayer
    {
        [Key]
        public int Id { get; set; }

        public Game Game { get; set; }

        [Column("player_name")]
        public string PlayerName { get; set; }

        [Column("wins")]
        public int Wins { get; set; }

        [Column("loses")]
        public int Loses { get; set; }

        [Column("round_position")]
        public int RoundPosition { get; set; }

        public GamePlayer(Game game, string playerName, int roundPosition)
        {
            this.Game = game;
            this.PlayerName = playerName;
            Wins = 0;   
            Loses = 0;  
            this.RoundPosition = roundPosition; 
        }


    }
}
