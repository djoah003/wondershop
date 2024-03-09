using System;
using System.Collections.Generic;
using System.Linq;

public struct PlayerPoints
{
    public PlayerPoints(int playerIndex, int points, PointConfig config, int round)
    {
        Player = playerIndex;
        Points = points;
        Rounds = round;
        Config = config;
    }

    public int Player { get; }
    public int Points { get; }
    public int Rounds { get; }
    public PointConfig Config { get; }
}

internal class PointsConfigComparer : IEqualityComparer<PointConfig>
{
    public int GetHashCode(PointConfig config) => config.id;

    public bool Equals(PointConfig config, PointConfig other)
    {
        if (config == null && other == null) return true;
        return config != null && other != null && config.id == other.id;
    }
}

public class PointManager : ProjectSingleton<PointManager>
{
    private readonly List<PlayerPoints> _playerPoints = new();
    //TODO: IS THIS CORRECT PLACE FOR THIS?
    private int[] _gameScores;

    /**
     * Reading interface
     *
     */
    private static bool WhereTotalScore(PlayerPoints points, int playerIndex, int pointsId, int round) =>
        // filter the scores for this player from the previous rounds
        (points.Player == playerIndex && points.Rounds < round) ||
        // filter the scores for this player from the current round up until the given point category
        (points.Player == playerIndex && points.Rounds == round && points.Config.id <= pointsId);

    public int GetTotalScore(int playerIndex, int pointsId, int round)
        => _playerPoints.Where(score => WhereTotalScore(score, playerIndex, pointsId, round))
            .Aggregate(0, (total, next) => total + next.Points);

    private static bool WhereRoundScore(PlayerPoints points, int playerIndex, int pointsId, int round) =>
        points.Player == playerIndex && points.Config.id == pointsId && points.Rounds == round;

    public int GetRoundScore(int playerIndex, int pointsId, int round)
        => _playerPoints.Where(score => WhereRoundScore(score, playerIndex, pointsId, round))
            .Aggregate(0, (total, next) => total + next.Points);

    public int GetScore(int index) => _gameScores != null && index < _gameScores.Length ? _gameScores[index] : 0;


    /**
     * Writing interface
     *
     */
    private void SetPoints(PlayerPoints points) => _playerPoints.Add(points);

    public void AddPoints(int playerIndex, int points, PointConfig config, int round) =>
        SetPoints(new PlayerPoints(playerIndex, points, config, round));

    public void SetScoreCount(int amount) => _gameScores = new int[amount];
    public void SetScore(int index, int value) => _gameScores[index] = value;
    public void AddToScore(int index, int value) => _gameScores[index] += value;

    public void ClearPoints() => _playerPoints.Clear();
    public void ClearScores() => _gameScores = Array.Empty<int>();
}