using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using winner.Interfaces;

namespace winner.Services
{
  public class PlayerService : IPlayerService
  {
    private List<Player> _players;
    private readonly ILogger<PlayerService> _logger;
    public PlayerService(ILogger<PlayerService> logger)
    {
      _logger = logger;
      _players = new List<Player>();
    }

    public List<Player> GetPlayers()
    {
      _logger.LogInformation($"Starting {nameof(PlayerService)} - {nameof(GetPlayers)} operation");
      return _players;
    }

    public bool LoadPlayers(Dictionary<string, string> playersData)
    {
      _logger.LogInformation($"Starting {nameof(PlayerService)} - {nameof(LoadPlayers)} operation");

      playersData.ToList().ForEach(playerData =>
      {
        _players.Add(new Player
        {
          Name = playerData.Key
        });

      });

      return true;
    }
  }
}
