using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigherOrLowerData.Entities
{
    [Table("player_moves")]
    public class PlayerMove
    {
        [Key]
        public uint Id { get; set; }

        [Column("previous_card")]
        [Required]
        public string PreviousCard{ get; set; }

        [Column("is_higher")]
        [Required]
        public bool isHigher { get; set; }


        [Column("next_card")]
        [Required]
        public string NextCard { get; set; }

        [Column("is_correct")]
        [Required]
        public bool IsCorrect { get; set; }



        [Column("round_count")]
        [Required]
        public int RoundCount { get; set; }



        [Column("player_id")]
        [Required]
        public uint PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public GamePlayer? GamePlayer { get; set; }

        [Column("player_name")]
        [Required]
        public string PlayerName { get; set; }


        [Column("game_id")]
        [Required]
        public uint GameId { get; set; }

        [ForeignKey("GameId")]
        public Game? Game { get; set; }


        public PlayerMove(uint gameId, uint playerId,  string playerName, string previousCard, bool isHigher, string nextCard, bool isCorrect, int roundCount)
        {            
            this.GameId = gameId;
            this.PlayerId = playerId;
            this.PlayerName = playerName;
            this.PreviousCard = previousCard;
            this.isHigher = isHigher;
            this.NextCard = nextCard;
            this.IsCorrect = isCorrect;
            this.RoundCount = roundCount;
        }

        


    }
}
