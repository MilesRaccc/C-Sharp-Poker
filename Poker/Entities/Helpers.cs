using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards = System.Collections.Generic.List<Poker.Entities.Card>;

namespace Poker.Entities
{
    public class PlayerAction
    {
        public ActionType Type { get; set; }
        public int Amount { get; set; } // Только для Raise или AllIn

        public static PlayerAction Fold() => new PlayerAction { Type = ActionType.Fold };
        public static PlayerAction Call() => new PlayerAction { Type = ActionType.Call };
        public static PlayerAction Check() => new PlayerAction { Type = ActionType.Check };
        public static PlayerAction Raise(int amount) => new PlayerAction { Type = ActionType.Raise, Amount = amount };
        public static PlayerAction AllIn(int totalChips) => new PlayerAction { Type = ActionType.AllIn, Amount = totalChips };
    }

    /// <summary>
    /// Result of evaluating a combination of 5 cards
    /// </summary>
    public class HandEvaluationResult
    {
        /// <summary>
        /// Matching Combo
        /// </summary>
        public Combos Combo { get; set; }
        /// <summary>
        /// Estimated strength of the card combination
        /// </summary>
        public double Strength { get; set; } // Например, 0.0 — мусор, 1.0 — натсы
        /// <summary>
        /// Checked cards
        /// </summary>
        public Cards Cards { get; set; } 
    }

    /// <summary>
    /// Collection of usefull methods for checking if presented cards match certain combinations.
    /// </summary>
    public static class CombinationHelper
    {
        /// <summary>
        /// Generates all unique 5-card combinations from the given list of <see cref="Card"/>s.
        /// </summary>
        /// <param name="cards">
        /// A collection of cards to generate combinations from. Must contain at least 5 <see cref="Card"/>s.
        /// </param>
        /// <returns>
        /// An enumerable sequence of 5-card combinations, where each combination is represented as a <see cref="Card"/> list.
        /// </returns>
        /// <remarks>
        /// This method performs a combinatorial selection (n choose 5) without repetitions.
        /// It is primarily used to evaluate all possible 5-card poker hands
        /// from a larger set of cards (for example, 7 cards in Texas Hold'em).
        /// </remarks>
        public static IEnumerable<Cards> GetAll5CardCombinations(Cards cards)
        {
            if (cards.Count < 5)
                yield break;

            int n = cards.Count;
            for (int i = 0; i < n - 4; i++)
            {
                for (int j = i + 1; j < n - 3; j++)
                {
                    for (int k = j + 1; k < n - 2; k++)
                    {
                        for (int l = k + 1; l < n - 1; l++)
                        {
                            for (int m = l + 1; m < n; m++)
                            {
                                yield return new Cards{ cards[i], cards[j], cards[k], cards[l], cards[m] };
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the cards match the combination "Straight"
        /// </summary>
        /// <param name="cards">List of <see cref="Card"/> to check for match</param>
        /// <param name="isLowAce">If match found - returns, if the Ace in combination on the low end.</param>
        /// <returns>Is there a match</returns>
        public static bool IsStraight(Cards cards, out bool isLowAce)
        {
            isLowAce = false;

            var values = cards.Select(card => (int)card.Value).Distinct().ToList();

            if (values.Count != 5) return false;

            if (values[0] == 2 && values[1] == 3 && values[2] == 4 && values[3] == 5 && values[4] == 14)
            {
                isLowAce = true;
                return true;
            }

            return values.Max() - values.Min() == 4;
        }

        /// <summary>
        /// Check if the cards match the combinations of "Pair" or "Set", depending on the "n" parameter.
        /// </summary>
        /// <param name="groups">List of Value+Card groups</param>
        /// <param name="n">Amount of card with the same value that needs to be determined</param>
        /// <returns>Is there a match</returns>
        public static bool HasNOfAKind(IList<IGrouping<Value,Card>> groups, int n)
        {
            return groups.Any(g => g.Count() == n);
        }

        /// <summary>
        /// Check if the cards match the combination "Full House"
        /// </summary>
        /// <param name="groups">List of Value+Card groups</param>
        /// <returns>Is there a match</returns>
        public static bool HasFullHouse(IList<IGrouping<Value,Card>> groups)
        {
            return groups.Count == 2 &&
                    (groups.Any(g => g.Count() == 2) && groups.Any(g => g.Count() == 3));
        }

        /// <summary>
        /// Check if the cards match the combination "Two Pair"
        /// </summary>
        /// <param name="groups">List of Value+Card groups</param>
        /// <returns>Is there a match</returns>
        public static bool HasTwoPair(IList<IGrouping<Value,Card>> groups)
        {
            return groups.Count(g => g.Count() == 2) == 2;
        }
    }

    /// <summary>
    /// Collection of usefull methods for evaluating cards, their combinations and strength.
    /// </summary>
    public static class PokerHandEvaluator
    {
        /// <summary>
        /// Based on provided state of the <see cref="Table"/> and <see cref="PlayerHand"/>, evaluates possible combinations, returning the best one.
        /// </summary>
        /// <param name="table">Current state of the game table</param>
        /// <param name="hand">Current player's hand</param>
        /// <returns>Evaluation result, containing best card combination, name of the combination and it's strength</returns>
        public static HandEvaluationResult Evaluate(Table table, PlayerHand hand)
        {
            var allCards = new Cards();
            allCards.AddRange(hand.hand);
            allCards.AddRange(table.ShownCards);

            var combinations = CombinationHelper.GetAll5CardCombinations(allCards);
            var result = new HandEvaluationResult() { Strength = 0 };

            foreach (var combo in combinations)
            {
                 var singleCombo = EvaluateSingleHand(combo);
                if (singleCombo.Strength > result.Strength)
                {
                    result = singleCombo;
                }
            }

            return result;
        }

        /// <summary>
        /// Evaluate method for test purposes
        /// </summary>
        /// <param name="allCards">A combination of 5~7 cards</param>
        /// <returns>Evaluation result</returns>
        public static HandEvaluationResult Evaluate(Cards allCards)
        {
            var combinations = CombinationHelper.GetAll5CardCombinations(allCards);
            var result = new HandEvaluationResult() { Strength = 0 };

            foreach (var combo in combinations)
            {
                var singleCombo = EvaluateSingleHand(combo);
                if (singleCombo.Strength > result.Strength)
                {
                    result = singleCombo;
                }
            }

            return result;
        }

        /// <summary>
        /// Evaluates a 5-card poker hand and returns its rank and calculated strength.
        /// </summary>
        /// <param name="cards">Exactly 5 cards to evaluate</param>
        /// <returns>A <see cref="HandEvaluationResult"/> containing the hand rank, cards that were evaluated, and numerical strength</returns>
        /// <exception cref="ArgumentException">Thrown if the number of cards is not exactly 5.</exception>
        public static HandEvaluationResult EvaluateSingleHand(Cards cards)
        {
            var result = new HandEvaluationResult();

            if (cards.Count != 5) throw new ArgumentException("Hand must contain exactly 5 cards!");

            var ordered = cards.OrderBy(card => (int)card.Value).ToList();
            var values = ordered.GroupBy(card => card.Value).ToList();

            bool isFlush = ordered.GroupBy(card => card.Suit).Count() == 1;
            bool isStraight = CombinationHelper.IsStraight(ordered, out bool isLowAce);

            var rankChecks = new (Func<bool> condition, Combos rank)[]
            {
                (() => isFlush && isStraight && ordered.Min(c => (int)c.Value) == (int)Value.Ten, Combos.Royal_Flush),
                (() => isFlush && isStraight, Combos.Straight_Flush),
                (() => CombinationHelper.HasNOfAKind(values, 4), Combos.Kare),
                (() => CombinationHelper.HasFullHouse(values), Combos.Full_House),
                (() => isFlush, Combos.Flush),
                (() => isStraight, Combos.Straight),
                (() => CombinationHelper.HasNOfAKind(values, 3), Combos.Set),
                (() => CombinationHelper.HasTwoPair(values), Combos.Two_Pair),
                (() => CombinationHelper.HasNOfAKind(values, 2), Combos.Pair)
            };

            result.Combo = rankChecks.FirstOrDefault(rc => rc.condition()).rank;
            result.Combo = result.Combo == 0 ? Combos.High_Card : result.Combo;
            result.Cards = cards;
            result.Strength = CalculateStrengthScore(result);

            return result;
        }

        /// <summary>
        /// Calculates <see cref="HandEvaluationResult"/> strength.
        /// </summary>
        /// <remarks>
        /// Takes the <see cref="HandEvaluationResult.Combo"/> number, adds values of the <see cref="HandEvaluationResult.Cards"/> in ascending order, multiplied by 14^iterator.
        /// </remarks>
        /// <param name="result">Current <see cref="HandEvaluationResult"/></param>
        /// <returns>Hand strength</returns>
        private static double CalculateStrengthScore(HandEvaluationResult result)
        {
            int baseScore = (int)result.Combo * 1000000;
            int kickerScore = 0;
            int multiplier = 1;

            foreach (var card in result.Cards.OrderBy(c => (int)c.Value))
            {
                kickerScore += (int)card.Value * multiplier;
                multiplier *= 14;
            }

            int totalScore = baseScore + kickerScore;
            return totalScore / 10000000.0; // нормализация
        }

        /// <summary>
        /// Evaluate first 2 cards that player gets from dealer
        /// </summary>
        /// <param name="firstCard"></param>
        /// <param name="secondCard"></param>
        /// <returns>Bet index for bots</returns>
        public static double EvaluatePreflop(Card firstCard, Card secondCard)
        {
            bool sameSuit = firstCard.Suit == secondCard.Suit;
            int high = Math.Max((int)firstCard.Value, (int)secondCard.Value);
            int low = Math.Min((int)firstCard.Value, (int)secondCard.Value);

            bool isPair = firstCard.Value == secondCard.Value;

            if (isPair)
            {
                if (high >= (int)Value.Ten) return 0.95; // премиум-пара
                if (high >= (int)Value.Six) return 0.75;
                return 0.55;
            }

            if (sameSuit && high >= (int)Value.King && low >= (int)Value.Ten)
                return 0.8;

            if (high >= (int)Value.Ace && low >= (int)Value.Jack)
                return 0.7;

            if (sameSuit && high >= (int)Value.Jack)
                return 0.6;

            return 0.3; // мусор
        }
    }
}
