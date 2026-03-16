using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Cards = System.Collections.Generic.List<Poker.Entities.Card>;
using Bots = System.Collections.Generic.List<Poker.Entities.IPlayerAI>;

namespace Poker.Entities
{
    #region Interfaces
    public interface IHand
    {
        Cards cards { get; set; }
        Combos currentCombo { get; set; }
    }

    public interface IPlayerAI
    {
        PlayerAction DecideAction(GameState gameState, PlayerState playerState);
    }

    #endregion

    /// <summary>
    /// Class representing a single card in deck
    /// </summary>
    /// <param name="Suit">Card Suit</param>
    /// <param name="Value">Card Value</param>
    public record Card(Suit Suit, Value Value)
    {
        public static List<string> DrawCardBack()
        {
            return new List<string>
            {
                "┌───────┐",
                "│╬╬╬╬╬╬╬│",
                "│╬░░░░░╬│",
                "│╬░░░░░╬│",
                "│╬░░░░░╬│",
                "│╬╬╬╬╬╬╬│",
                "└───────┘"
            };
        }

        public List<string> ConvertToString() 
        {
            char symbol = '_';

            //StringBuilder sb = new StringBuilder();
            //if()
            //{
            //    sb.Append((int)Value);
            //}
            //else
            //{
            //    sb.Append(Value.ToString());
            //}

            //sb.Append(" of " + Suit.ToString());

            switch (Suit)
            {
                case Suit.Heart:
                    symbol = '♥';
                    break;
                case Suit.Diamond:
                    symbol = '♦';
                    break;
                case Suit.Club:
                    symbol = '♣';
                    break;
                case Suit.Spade:
                    symbol = '♠';
                    break;
                default:
                    break;
            }

            
            List<string> cardConsoleUI = new();
            string value = "??";

            switch (Value)
            {
                case Value.Jack:
                    value = "J"; break;
                case Value.Queen:
                    value = "Q"; break;
                case Value.King:
                    value = "K"; break;
                case Value.Ace:
                    value = "A"; break;
                default:
                    value = ((int)Value).ToString();
                    break;
            }

            cardConsoleUI.Add("┌───────┐");
            cardConsoleUI.Add($"│{(Value == Value.Ten ? value : value + " ")}     │");
            cardConsoleUI.Add("│       │");
            cardConsoleUI.Add($"│   {symbol}   │");
            cardConsoleUI.Add("│       │");
            cardConsoleUI.Add($"│     {(Value == Value.Ten ? value : " " + value)}│");
            cardConsoleUI.Add("└───────┘");

            return cardConsoleUI;

            //return sb.ToString();
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
            return cards.ToString();
        }

        public int Count()
        {
            return this.cards.Count;
        }

        public void Shuffle()
        {
            for (int i = this.cards.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(0, i + 1);
                (this.cards[i], this.cards[j]) = (this.cards[j], this.cards[i]);
            }
        }

        public Card Draw()
        {
            var index = Random.Shared.Next(0, this.cards.Count);
            var card = this.cards[index];
            this.cards.RemoveAt(index);
            return card;
        }
    }

    public class Cards : IList<Card>
    {
        IList<Card> cards = [];

        public IEnumerator<Card> GetEnumerator()
        {
            return cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region Implementation of ICollection<T>

        public void Add(Card item)
        {
            cards.Add(item);
        }

        public void Clear()
        {
            cards.Clear();
        }

        public bool Contains(Card item)
        {
            return cards.Contains(item);
        }

        public void CopyTo(Card[] array, int arrayIndex)
        {
            cards.CopyTo(array, arrayIndex);
        }

        public bool Remove(Card item)
        {
            return cards.Remove(item);
        }

        public int Count
        {
            get { return cards.Count; }
        }

        public bool IsReadOnly
        {
            get { return cards.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<T>

        public int IndexOf(Card item)
        {
            return cards.IndexOf(item);
        }

        public void Insert(int index, Card item)
        {
            cards.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            cards.RemoveAt(index);
        }

        public void AddRange(IEnumerable<Card> newList)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            ArgumentNullException.ThrowIfNull(newList);

            if (cards is List<Card> concreteList)
            {
                concreteList.AddRange(newList);
                return;
            }

            foreach (var element in newList)
            {
                if (element is Card card)
                {
                    cards.Add(card);
                }
            }
        }

        public Card this[int index]
        {
            get { return cards[index]; }
            set { cards.Insert(index, value); }
        }

        public List<Card> GetCards()
        {
            return (List<Card>)(cards ?? new List<Card>());
        }

        #endregion

        #region CustomConsoleOutput

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (cards.Count > 8)
            {
                for (int i = 0; i < cards.Count; i += 4)
                {
                    List<string> consoleOutput = [];

                    for (int j = i; j < i + 4; j++)
                    {
                        if (j == i)
                        {
                            consoleOutput = cards[j].ConvertToString();
                        }
                        else if (j < cards.Count)
                        {
                            var cardStrings = cards[j].ConvertToString();

                            for (int k = 0; k < cardStrings.Count; k++)
                            {
                                consoleOutput[k] = consoleOutput[k].Replace(consoleOutput[k], consoleOutput[k] + " " + cardStrings[k]);
                            }
                        }


                    }

                    sb.Append(String.Join("\n", consoleOutput) + "\n");

                }
            }
            else
            {
                List<string> consoleOutput = [];

                for (int i = 0; i < cards.Count; i++)
                {
                    if (i == 0)
                    {
                        consoleOutput = cards[i].ConvertToString();
                    }
                    else
                    {
                        var cardStrings = cards[i].ConvertToString();

                        for (int k = 0; k < cardStrings.Count; k++)
                        {
                            consoleOutput[k] = consoleOutput[k].Replace(consoleOutput[k], consoleOutput[k] + " " + cardStrings[k]);
                        }
                    }
                }

                sb.Append(String.Join("\n", consoleOutput) + "\n");
            }

            return sb.ToString();
        }

        #endregion
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

    public class PlayerHand(Cards cards, Combos currentCombo) : IHand
    {
        public Cards cards { get; set; } = cards;
        public Combos currentCombo { get; set; } = currentCombo;
    }

    public class AiPlayer : IPlayerAI
    {
        public PlayerAction DecideAction(GameState gameState, PlayerState playerState)
        {
            var table = gameState.Table;

            var result = PokerHandEvaluator.Evaluate(table, playerState.Hand);

            var strength = result.Strength;

            int amountToCall = table.CurrentBet;

            // очень сильная рука
            if (strength > 0.7)
            {
                return PlayerAction.Raise(table.MinRaise * 3);
            }

            // сильная рука
            if (strength > 0.6)
            {
                if (strength < 0.65)
                    return PlayerAction.Call();
                else
                    return PlayerAction.Raise(table.MinRaise * 2);
            }

            // средняя
            if (strength > 0.4)
            {
                if (amountToCall == 0)
                    return PlayerAction.Check();

                return PlayerAction.Call();
            }

            // слабая
            if (strength < 0.3 && amountToCall > table.BigBlind)
            {
                return PlayerAction.Fold();
            }

            return PlayerAction.Call();
        }
    }

    public class PlayerState(PlayerHand hand, string? name = null, int chips = 10000, IPlayerAI? Controller = null)
    {
        public string Name { get; set; } = name ?? "MidBot" + Random.Shared.Next(100).ToString();
        public PlayerHand Hand { get; set; } = hand;
        public int Chips { get; set; } = chips;
        public int CurrentBet { get; set; } = 0;
        public bool IsFolded { get; set; } = false;
        public bool IsAllIn { get; set; } = false;
        public IPlayerAI Controller { get; set; } = Controller;
        public bool IsHuman => Controller == null;
    }

    public class GameState(Deck deck, List<PlayerState> players, Table table, int currentMainPlayerIndex)
    {
        public Deck Deck { get; set; } = deck;
        public List<PlayerState> Players { get; set; } = players;
        public Table Table { get; set; } = table;
        public int CurrentMainPlayerIndex { get; set; } = currentMainPlayerIndex;
        public GamePhase Phase { get; set; }

        public void PreFlop()
        {
            Phase = GamePhase.PreFlop;
        }

        public void Flop()
        {
            Phase = GamePhase.Flop;
        }

        public void Turn()
        {
            Phase = GamePhase.Turn;
        }

        public void River()
        {
            Phase = GamePhase.River;
        }

        public void Showdown()
        {
            Phase = GamePhase.Showdown;
        }
    }

    public static class PokerGame
    {
        public static (bool, int) GameStart(string playerName, int AiAmount, int currentMainPlayerIndex)
        {
            var players = new List<PlayerState>();
            var deck = new Deck();
            var table = new Table(new Cards(), 100, 0);
            var humanPlayer = new PlayerState(new PlayerHand([], Combos.None), playerName);
            players.Add(humanPlayer);

            for (int i = 0; i < AiAmount; i++) 
            {
                var AiPlayer = new PlayerState(hand: new PlayerHand([], Combos.None), Controller: new AiPlayer());
                players.Add(AiPlayer);
            }

            var gameState = new GameState(deck, players, table, currentMainPlayerIndex);

            deck.Shuffle();

            foreach (var player in players)
            {
                player.Hand.cards.Add(deck.Draw());
                player.Hand.cards.Add(deck.Draw());
            }

            string? response = null;

            while (response == null)
            {
                Console.WriteLine("Another round? Y/N");
                response = Console.ReadLine();
                if (response != null && response.ToUpper() == "Y")
                {
                    return (true, 0);
                }
                else
                {
                    break;
                }
            }

            return (false, humanPlayer.Chips);
        }

        private static void Betting(GameState state)
        {
            var players = state.Players;
            var table = state.Table;

            int playersActed = 0;
            int lastPlayerActed = state.CurrentMainPlayerIndex;

            while (true)
            {
                var player = players[lastPlayerActed];

                if (!player.IsFolded && !player.IsAllIn)
                {
                    PlayerAction action;

                    if (player.IsHuman)
                    {
                        action = null;//GetHumanAction(state, player);
                    }
                    else
                    {
                        action = player.Controller!.DecideAction(state, player);
                    }

                    ApplyAction(state, player, action);

                    playersActed++;
                }

                if (IsBettingFinished(state, playersActed))
                    break;

                 lastPlayerActed++;
            }

            ResetPlayerBets(players);
        }

        private static bool IsBettingFinished(GameState state, int playersActed)
        {
            var activePlayers = state.Players
                .Where(p => !p.IsFolded && !p.IsAllIn)
                .ToList();

            if (activePlayers.Count <= 1)
                return true;

            bool allEqual = activePlayers
                .All(p => p.CurrentBet == state.Table.CurrentBet);

            return allEqual && playersActed >= activePlayers.Count;
        }

        private static void ApplyAction(GameState state, PlayerState player, PlayerAction action)
        {
            var table = state.Table;

            switch (action.Type)
            {
                case ActionType.Fold:
                    player.IsFolded = true;
                    break;

                case ActionType.Check:
                    break;

                case ActionType.Call:
                    {
                        int amount = table.CurrentBet - player.CurrentBet;

                        player.Chips -= amount;
                        player.CurrentBet += amount;
                        table.Pot += amount;

                        break;
                    }

                case ActionType.Raise:
                    {
                        int raise = action.Amount;

                        int total = table.CurrentBet + raise;
                        int amount = total - player.CurrentBet;

                        player.Chips -= amount;
                        player.CurrentBet = total;

                        table.CurrentBet = total;
                        table.Pot += amount;

                        break;
                    }

                case ActionType.AllIn:
                    {
                        int amount = player.Chips;

                        player.CurrentBet += amount;
                        table.Pot += amount;

                        player.Chips = 0;
                        player.IsAllIn = true;

                        if (player.CurrentBet > table.CurrentBet)
                            table.CurrentBet = player.CurrentBet;

                        break;
                    }
            }
        }

        private static void ResetPlayerBets(List<PlayerState> players)
        {
            foreach (var player in players)
            {
                player.CurrentBet = 0;
            }
        }
    }

}
