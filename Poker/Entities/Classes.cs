using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards = System.Collections.Generic.List<Poker.Entities.Card>;
using Bots = System.Collections.Generic.List<Poker.Entities.IPlayerAI>;

namespace Poker.Entities
{
    #region Interfaces
    public interface IHand
    {
        Cards hand { get; set; }
        Combos currentCombo { get; set; }
    }

    public interface IPlayerAI
    {
        PlayerAction DecideAction(GameState gameState,PlayerHand hand);
    }

    #endregion

    /// <summary>
    /// Class representing a single card in deck
    /// </summary>
    /// <param name="Suit">Card Suit</param>
    /// <param name="Value">Card Value</param>
    public record Card(Suit Suit, Value Value)
    {
        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            if((int)Value > 1 && (int)Value < 11)
            {
                sb.Append((int)Value);
            }
            else
            {
                sb.Append(Value.ToString());
            }

            sb.Append(" of " + Suit.ToString());

            //switch (Suit)
            //{
            //    case Suit.Hearts:
            //        sb.Append("♥");
            //        break;
            //    case Suit.Diamonds:
            //        sb.Append("♦");
            //        break;
            //    case Suit.Clubs:
            //        sb.Append("♣");
            //        break;
            //    case Suit.Spade:
            //        sb.Append("♠");
            //        break;
            //    default:
            //        break;
            //}

            return sb.ToString();
        }
    }

    /// <summary>
    /// Class representing a card deck.
    /// </summary>
    public class Deck
    {
        Cards cards {  get; set; }
        public Deck()
        {
            this.cards = [];

            for (int i = 0; i < Enum.GetValues(typeof(Suit)).Length; i++) 
            {
                for(int j = 2; j < Enum.GetValues(typeof(Value)).Length + 2; j++)
                {
                    cards.Add(new Card((Suit)i, (Value)j));
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var card in cards) 
            {
                sb.Append(card.ToString() + "\n");
            }
            return sb.ToString();
        }

        public int Count()
        {
            return cards.Count;
        }

        public void Shuffle()
        {

        }

        public Card Draw()
        {
            return new Card(Suit.Heart,Value.Ace);
        }
    }

    /// <summary>
    /// A class representing current Table state
    /// </summary>
    /// <param name="shownCards">Current cards on the table</param>
    /// <param name="smallBlind">Starting bet for a table</param>
    /// <param name="minRaise">Minimum raise amount for a table</param>
    public class Table(Cards shownCards, int smallBlind,  int minRaise)
    {
        public Cards ShownCards { get; set; } = shownCards;
        public int Pot { get; set; } = 0;
        public int CurrentBet { get; set; } = 0;
        public int BetToCall { get; set; } = 0; // Сколько нужно заколлировать
        public int MinRaise { get; set; } = minRaise; // Минимальный рейз

        public int BigBlind { get; set; } = smallBlind * 2;
    }

    public class PlayerHand(Cards hand, Combos currentCombo) : IHand
    {
        public Cards hand { get; set; } = hand;
        public Combos currentCombo { get; set; } = currentCombo;
    }

    public class AiPlayer : IPlayerAI
    {
        public PlayerAction DecideAction(GameState gameState, PlayerHand hand)
        {
            return PlayerAction.Fold();
        }
    }

    public class PlayerState(PlayerHand hand, string? name = null, int chips = 10000, IPlayerAI? Controller = null)
    {
        public string Name { get; set; } = name ?? "MidBot" + Random.Shared.Next(100).ToString();
        public PlayerHand Hand { get; set; } = hand;
        public int Chips { get; set; } = chips;
        public bool IsFolded { get; set; } = false;
        public bool IsAllIn { get; set; } = false;

        public bool IsHuman => Controller == null;
    }

    public class GameState(Deck deck, List<PlayerState> players, Table table)
    {
        public Deck Deck { get; set; } = deck;
        public List<PlayerState> Players { get; set; } = players;
        public Table Table { get; set; } = table;
        public int CurrentPlayerIndex { get; set; }
        public GamePhase Phase { get; set; }
    }

    public static class PokerGame
    {
        public static void GameStart(string playerName, int AiAmount)
        {
            var players = new List<PlayerState>();
            var humanPlayer = new PlayerState(new PlayerHand([], Combos.None), playerName);
            players.Add(humanPlayer);

            for (int i = 0; i < AiAmount; i++) 
            {
                var AiPlayer = new PlayerState(hand: new PlayerHand([], Combos.None));
                players.Add(AiPlayer);
            }



        }
    }

}
