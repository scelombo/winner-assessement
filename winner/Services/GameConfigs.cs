using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using winner.Interfaces;

namespace winner.Services
{
  public class GameConfigs : IGameConfigs
  {
    private readonly IConfiguration _config;
    public GameConfigs(IConfiguration config)
    {
      _config = config;
    }
    public string InputFileKey
    {
      get { return _config[nameof(InputFileKey)]; }
    }

    public string OutPutFileKey
    {
      get { return _config[nameof(OutPutFileKey)]; }
    }

    public Dictionary<string, int> CardValues
    {
      get { return _config.GetSection(nameof(CardValues)).Get<Dictionary<string, int>>(); }
    }
    public Dictionary<string, int> SuitScores
    {
      get { return _config.GetSection(nameof(SuitScores)).Get<Dictionary<string, int>>(); }
    }
  }
}
