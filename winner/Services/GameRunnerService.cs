using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using winner.Interfaces;

namespace winner.Services
{
  public class GameRunnerService : IGameRunnerService
  {

    private readonly ILogger<GameRunnerService> _logger;
    private readonly IGameConfigs _gameConfigs;
    private readonly IGameService _gameService;
    private readonly IPlayerService _playerService;
    private Dictionary<string, string> _gameInputCommands;
    public GameRunnerService(ILogger<GameRunnerService> logger, 
                             IGameConfigs gameConfigs,
                             IGameService gameService,
                             IPlayerService playerService)
    {
      _logger = logger;
      _gameConfigs = gameConfigs;
      _gameService = gameService;
      _playerService = playerService;
    }

    public void Run(string[] args)
    {
      try
      {

        _logger.LogInformation($"Starting {nameof(GameRunnerService)} - {nameof(Run)} operation", args);

        if (!IsValidGameInputCommands(args))
          ExitWithFailOverError("Game input commands invalid.");

        _gameInputCommands = TransposeArgsToDictionary(args);

        string inputFileName = string.Empty;

        if (IsInputFileValid())
          inputFileName = _gameInputCommands[_gameConfigs.InputFileKey];

        _ = IsOutPutFileValid();

        _logger.LogInformation($"Initializing game datasource");
        var dataSource = File.ReadAllLines(@$"{inputFileName}");
        var gameDataSource = dataSource.ToDictionary(t => t.Split(':')[0], y => y.Split(':')[1]);

        _logger.LogInformation($"Delegating to {nameof(IPlayerService)} - IOC");
        _playerService.LoadPlayers(gameDataSource);

        _logger.LogInformation($"Delegating to {nameof(IGameService)} - IOC");
        _gameService.Setup(_playerService.GetPlayers(), gameDataSource);
        _gameService.Play();

        WriteGameResultsOutput(_gameService.Draw());

      }
      catch (Exception ex)
      {
        _logger.LogError("Application failed and Game aborted", ex);
        ExitWithFailOverError(ex.Message);
      }

      Console.ReadLine();
    }

    private void ExitWithFailOverError(string exitError)
    {
      _logger.LogError($"Application exited with error : {exitError}");
      if (_gameInputCommands != null && _gameInputCommands.Any(c => c.Key == _gameConfigs.OutPutFileKey))
      {
        File.WriteAllText(@$"{_gameInputCommands[_gameConfigs.OutPutFileKey]}", "ERROR");
        _logger.LogInformation("Game error output was created");
      }
      else
        _logger.LogError($"Game output file parameter missing, output file couldn't be generated.");

      Console.WriteLine("Press any command to exit.");
      Console.ReadLine();
      Environment.Exit(-1);
    }

    private void WriteGameResultsOutput(string winners)
    {
      _logger.LogInformation($"Writing out game output results to output file..");
      File.WriteAllText(@$"{_gameInputCommands[_gameConfigs.OutPutFileKey]}", winners);
      _logger.LogInformation("Game results output file was created");

      Console.ReadLine();
    }
    private bool IsValidGameInputCommands(string[] args)
    {
      _logger.LogInformation($"Validating game input arguments", args);

      if (!args.Any())
      {
        _logger.LogError($"No arguments list provided");
        return false;
      }        

      if (args.Length % 2 > 0)
      {
        _logger.LogError($"Incorrect value pair arguments list provided");
        return false;
      }

      return true;
    }

    public Dictionary<string, string> TransposeArgsToDictionary(string[] args)
    {
      _logger.LogInformation($"Transposing game input arguments to dictionary array", args);

      Dictionary<string, string> response = new Dictionary<string, string>();

      var maxInd = args.Length / 2;

      for (int ind = 0; ind <= maxInd;)
      {
        response.Add(args[ind].ToUpper(), args[++ind]);
        ind += 1;
      }

      return response;
    }

    private bool IsInputFileValid()
    {
      _logger.LogInformation($"Validating game input file name argument");

      if (!_gameInputCommands.ContainsKey(_gameConfigs.InputFileKey))
        ExitWithFailOverError($"Input file key [ {_gameConfigs.InputFileKey} ] was not fount.");

      string inputFileName = _gameInputCommands[_gameConfigs.InputFileKey];
      if (string.IsNullOrEmpty(inputFileName))
        ExitWithFailOverError($"Input file name value was not provided.");

      if (!File.Exists(inputFileName))
        ExitWithFailOverError($"Input file name [ {inputFileName} ] does not exists.");

      return true;
    }

    private bool IsOutPutFileValid()
    {
      _logger.LogInformation($"Validating game output file name argument");

      if (!_gameInputCommands.ContainsKey(_gameConfigs.OutPutFileKey))
        ExitWithFailOverError($"Output file key [ {_gameConfigs.OutPutFileKey} ] was not fount.");

      if (string.IsNullOrEmpty(_gameInputCommands[_gameConfigs.OutPutFileKey]))
        ExitWithFailOverError($"Output file name value was not provided.");

      return true;
    }

  }
}
