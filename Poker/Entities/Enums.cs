namespace Poker.Entities
{
    public enum Suit
    {
        Spade,
        Heart,
        Diamond,
        Club
    }

    public enum Value
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public enum Combos
    {
        None,
        High_Card,
        Pair,
        Two_Pair,
        Set,
        Straight,
        Flush,
        Full_House,
        Kare,
        Straight_Flush,
        Royal_Flush
    }

    public enum ActionType
    {
        Fold,
        Call,
        Raise,
        AllIn,
        Check
    }

    public enum GamePhase
    {
        PreFlop,
        Flop,
        Turn,
        River,
        Showdown
    }


}
