using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace winner
{
  public class Player
  {
    public string Name { get; set; }
    [MinLength(2), MaxLength(2)]
    public List<string> Hand { get; set; }
    public int ScoreFaceValue { get; set; }
    public int ScoreSuetValue { get; set; }
  }
}
