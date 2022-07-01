using System;
using System.Collections.Generic;
using System.Text;

namespace winner.Interfaces
{
  public interface IGameConfigs
  {
    string InputFileKey { get;}
    string OutPutFileKey { get;}
    Dictionary<string, int> CardValues { get; }
    Dictionary<string, int> SuitScores { get; }
  }
}
