using Microsoft.EntityFrameworkCore;
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
    [Index(nameof(GameId), nameof(PlayerName), IsUnique = true)]
    public class GamePlayer
    {
        [Key]
        public uint Id { get; set; }

        [Column("player_name")]
        [MaxLength(100)]
        public string PlayerName { get; set; }

        [Column("wins")]
        public int Wins { get; set; }

        [Column("loses")]
        public int Loses { get; set; }

        [Column("round_position")]
        public int RoundPosition { get; set; }

        [Column("game_id")]
        public uint GameId { get; set; }
        
        [ForeignKey("GameId")]
        public Game Game { get; set; }

            
        //public GamePlayer(uint gameId, string playerName, int roundPosition)
        //{
        //    this.GameId= gameId;
        //    this.PlayerName = playerName;
        //    Wins = 0;   
        //    Loses = 0;  
        //    this.RoundPosition = roundPosition; 
        //}
    }
}
