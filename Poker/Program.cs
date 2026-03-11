using Poker.Entities;

Deck deck = new Deck();
//Console.WriteLine(deck.ToString());
//Console.WriteLine(deck.Count());
//Console.WriteLine();

//var cards = new List<Card>
//{
//    new Card(Suit.Spade, Value.Ace),
//    new Card(Suit.Heart, Value.King),
//    new Card(Suit.Club, Value.Queen),
//    new Card(Suit.Diamond, Value.Jack),
//    new Card(Suit.Spade, Value.Ten),
//    new Card(Suit.Heart, Value.Nine),
//    new Card(Suit.Club, Value.Eight)
//};
//var combinations = CombinationHelper.GetAll5CardCombinations(cards).ToList();
//Console.WriteLine("Количество Комбинаций: " + combinations.Count); // Должно быть 21
//Console.WriteLine();
//Console.WriteLine("Список комбинаций:");
//foreach (var combo in combinations)
//{
//    Console.WriteLine(string.Join(",", combo));
//}
//Console.WriteLine();

//Console.WriteLine("Лучшая комбинация:");
//Console.WriteLine(string.Join(",", PokerHandEvaluator.Evaluate(cards).Cards));
//Console.WriteLine();
//Console.ReadLine();

Console.WriteLine("Wanna play a poker game? Y/N");
var response = Console.ReadLine();
response = response.ToUpper();

if (response == "Y")
{
    Console.WriteLine("Great, let's start. Tell me your name.");
    var playerName = Console.ReadLine();
    Console.WriteLine("Cool, now, how many AI's you want to play against?");
    var input = Console.ReadLine();
    while (true)
    {
        PokerGame.GameStart(playerName ?? "PissAnt", int.TryParse(input, out int AiAmount) ? AiAmount : 2);
    }
}