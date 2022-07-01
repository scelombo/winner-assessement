using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using winner.Interfaces;

namespace winner.Services
{
  public class GameService : IGameService
  {
    private List<Player> _players;
    private readonly IGameConfigs _gameConfigs;
    private readonly ILogger<GameService> _logger;
    public GameService(IGameConfigs gameConfigs, ILogger<GameService> logger)
    {
      _logger = logger;
      _gameConfigs = gameConfigs;
      _players = new List<Player>();
    }

    public bool Setup(List<Player> players, Dictionary<string, string> playersData)
    {
      _logger.LogInformation($"Starting {nameof(GameService)} - {nameof(Setup)} operation");

      _players = players;
      playersData.ToList().ForEach(playerData =>
      {
        _players.First(p => p.Name == playerData.Key).Hand = TryGetPlayerHand(playerData);
      });

      return true;
    }

    private List<string> TryGetPlayerHand(KeyValuePair<string, string> playerData)
    {
      var hand = playerData.Value.Split(',').ToList();
      _ = IsValidHand(hand, playerData.Key);
      return hand;
    }

    private bool IsValidHand(List<string> hand, string player)
    {
      if (hand.Any(card => card.Length > 2))
        throw new Exception($"Invalid hand dealt for player [{player}]");

      return true;
    }

    public bool Play()
    {
      _logger.LogInformation($"Starting {nameof(GameService)} - {nameof(Play)} operation");

      _players.ForEach(player =>
      {
        player.ScoreFaceValue = CalculateScore(player.Name, player.Hand, _gameConfigs.CardValues, Placement.Face);
        player.ScoreSuetValue = CalculateScore(player.Name, player.Hand, _gameConfigs.SuitScores, Placement.Suet);
      });

      _players = _players.OrderByDescending(p => p.ScoreFaceValue).ThenBy(p => p.ScoreSuetValue).ToList();

      return true;
    }

    public string Draw()
    {
      _logger.LogInformation($"Starting {nameof(GameService)} - {nameof(Draw)} operation");

      var faceValueScoreWinner = _players.OrderByDescending(p => p.ScoreFaceValue).ThenBy(p => p.ScoreSuetValue).First();
      var finalWinners = _players.Where(p => p.ScoreFaceValue == faceValueScoreWinner.ScoreFaceValue);

      if (finalWinners.Count() > 1)
      {
        _logger.LogInformation($"Tie detected, {finalWinners.Count()} players got a top face value score of {faceValueScoreWinner.ScoreFaceValue}");
        var suetValueScoreWinner = finalWinners.OrderByDescending(p => p.ScoreSuetValue).First();
        _logger.LogInformation($"Using suet scores to draw up the new winner(s)");

        finalWinners = finalWinners.Where(p => p.ScoreSuetValue == suetValueScoreWinner.ScoreSuetValue);
        _logger.LogInformation($"{finalWinners.Count()} new winer(s) returned with a top suet value score of {suetValueScoreWinner.ScoreFaceValue}");
      }

      var winnersOutputText = string.Empty;
      finalWinners.ToList().ForEach(winner =>
      {
        winnersOutputText += string.IsNullOrEmpty(winnersOutputText) ? winner.Name : $",{winner.Name}";
      });

      _logger.LogInformation($"Drawing up the winer(s)");

      var winner_s = winnersOutputText += $":{finalWinners.First().ScoreFaceValue}";

      _logger.LogInformation($"The winer(s) : {winner_s}");

      return winner_s;
    }


    private static int CalculateScore(string player, List<string> hand, Dictionary<string, int> scoreboard, Placement placement)
    {
      var score = 0;
      hand.ToList().ForEach(card =>
      {
        char val = placement == Placement.Face ? card.First() : card.Last();
        if (scoreboard.ContainsKey(val.ToString()))
          score += scoreboard[val.ToString()];
        else if (char.IsDigit(val))
          score += int.Parse(val.ToString());
        else
        {
          var valueType = placement == Placement.Face ? "Face" : "Suite";
          throw new Exception($"Invalid card [{card}] dealt to player [{player}]. [{val}] is not a valid [{valueType}]");
        }
          
      });

      return score;
    }

  }
}
