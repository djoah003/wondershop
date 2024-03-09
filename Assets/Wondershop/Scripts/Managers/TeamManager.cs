using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Team
{
    Red,
    Blue,
    Green,
    Yellow,
}

public struct TeamPlayer
{
    public readonly Team Team;
    public readonly GameObject Player;

    public TeamPlayer(Team team, GameObject player)
    {
        Team = team;
        Player = player;
    }
}

public class TeamManager : ProjectSingleton<TeamManager>
{
    private int _teamCount;
    private static readonly List<TeamPlayer> Teams = new List<TeamPlayer>();
    private static readonly Dictionary<Team, int> teamPoints = new ();

    private void OnDestroy()
    {
        if (Instance == this)
        {
            //Make sure static team list doesn't stay in memory after game ends
            Teams.Clear();
            teamPoints.Clear();
        }
    }

    /**
     * Team interface
     *
     */
    public void TeamsReset() => Teams.Clear();

    public static void TeamAddPlayer(Team team, GameObject player) => Teams.Add(new TeamPlayer(team, player));

    /**
     * Player interface
     *
     */
    public static Team PlayerTeam(GameObject player) => Teams.Last(teamPlayer => teamPlayer.Player == player).Team;

    public static List<GameObject> PlayersByTeam(Team team)
    {
        var teamPlayers = new List<GameObject>();
        foreach (TeamPlayer teamPlayer in Teams)
        {
            if (teamPlayer.Team == team)
            {
                //Add to team if team is correct
                if (!teamPlayers.Contains(teamPlayer.Player)) teamPlayers.Add(teamPlayer.Player);
            }
            else if (teamPlayers.Contains(teamPlayer.Player))
            {
                //Remove from team if not (this fill fix issues where the player is not currently in the team)
                teamPlayers.Remove(teamPlayer.Player);
            }
        }
        return teamPlayers;
    }
    
    public static int PlayerByTeamCount(Team team) => PlayersByTeam(team).Count;

    /**
     * Feature interface
     *
     */
    ///<summary>
    /// Method <c>CheckTeam</c> will return whether or not the players are in the same team 
    ///</summary>
    /// <returns>True if players are in the same team, False if not</returns>
    /// 
    public static bool CompareTeams(GameObject that, GameObject other) => PlayerTeam(that) == PlayerTeam(other);

    /// <summary>
    /// Add given points to the points of the given team
    /// </summary>
    /// <param name="team"></param>
    /// <param name="pointsToAdd"></param>
    /// <returns></returns>
    public static void AddTeamPoints(Team team, int pointsToAdd)
    {
        if (!teamPoints.TryAdd(team, pointsToAdd))
        {
            teamPoints[team] += pointsToAdd;
        }
    }

    public static int GetTeamPoints(Team team)
    {
        return teamPoints[team];
    }

    public static void ClearTeamPoints()
    {
        teamPoints.Clear();
    }
}