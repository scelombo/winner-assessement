using System.Collections.Generic;
using System.Threading.Tasks;

namespace winner.Interfaces
{
  public interface IPlayerService
  {
    bool LoadPlayers(Dictionary<string, string> playersData);
    List<Player> GetPlayers();
  }
}
