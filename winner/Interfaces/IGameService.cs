using System;
using System.Collections.Generic;
using System.Text;

namespace winner.Interfaces
{
  public interface IGameService
  {
    bool Setup(List<Player> players, Dictionary<string, string> playersData);
    bool Play();
    string Draw();

   
  }
}
