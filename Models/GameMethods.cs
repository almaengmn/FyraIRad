using Microsoft.Data.SqlClient;
using System.Data;

namespace FyraIRad.Models
{
    public class GameMethods
    {
        private readonly string _connectionString;


        public GameMethods(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<GameDetails> GetGameDetails(out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "SELECT * FROM Games";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataSet dataSet = new DataSet();

                List<GameDetails> GameList = new List<GameDetails>();

                try
                {
                    sqlConnection.Open();
                    sqlDataAdapter.Fill(dataSet, "GameList");

                    int i = 0;
                    int count = 0;

                    count = dataSet.Tables["GameList"].Rows.Count;

                    if (i < count)
                    {
                        while (i < count)
                        {
                            GameDetails Game = new GameDetails();
                            Game.GameId = Convert.ToInt32(dataSet.Tables["GameList"].Rows[i]["GameId"]);
                            Game.playerRedId = Convert.ToInt32(dataSet.Tables["GameList"].Rows[i]["playerRedId"]);
                            Game.playerYellowId = Convert.ToInt32(dataSet.Tables["GameList"].Rows[i]["playerYellowId"]);
                            Game.CurrentTurn = Convert.ToChar(dataSet.Tables["GameList"].Rows[i]["CurrentTurn"]);
                            Game.Status = Convert.ToString(dataSet.Tables["GameList"].Rows[i]["Status"]);

                            i++;
                            GameList.Add(Game);
                        }
                        errormsg = "";
                        return GameList;
                    }
                    else
                    {
                        errormsg = "No Games found in database";
                        return GameList;

                    }
                }
                catch (Exception e)
                {
                    errormsg = e.Message;
                    return GameList;
                }
            }


        }

        public GameDetails GetGameById(int gameId, out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "SELECT * FROM Games WHERE GameId=@GameId";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@GameId", gameId);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataSet dataSet = new DataSet();
                try
                {
                    sqlConnection.Open();
                    sqlDataAdapter.Fill(dataSet, "GameList");
                    int i = 0;
                    int count = 0;
                    count = dataSet.Tables["GameList"].Rows.Count;
                    if (i < count)
                    {
                        GameDetails Game = new GameDetails();
                        Game.GameId = Convert.ToInt32(dataSet.Tables["GameList"].Rows[i]["GameId"]);
                        Game.playerRedId = Convert.ToInt32(dataSet.Tables["GameList"].Rows[i]["playerRedId"]);
                        Game.playerYellowId = Convert.ToInt32(dataSet.Tables["GameList"].Rows[i]["playerYellowId"]);
                        Game.CurrentTurn = Convert.ToChar(dataSet.Tables["GameList"].Rows[i]["CurrentTurn"]);
                        Game.Status = Convert.ToString(dataSet.Tables["GameList"].Rows[i]["Status"]);
                        if (dataSet.Tables["GameList"].Rows[i]["Winner"] != DBNull.Value)
                        {
                            Game.Winner = Convert.ToChar(dataSet.Tables["GameList"].Rows[i]["Winner"]);
                        }
                        errormsg = "";
                        return Game;
                    }
                    else
                    {
                        errormsg = "No Games found in database";
                        return null;
                    }
                }
                catch (Exception e)
                {
                    errormsg = e.Message;
                    return null;
                }
            }
        }

        public void CreateGame(GameDetails newGame)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "INSERT INTO Games (PlayerRedId , PlayerYellowId, CurrentTurn, Status) VALUES (@PlayerRedId, @PlayerYellowId, @CurrentTurn, @Status )";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@PlayerRedId", newGame.playerRedId);
                sqlCommand.Parameters.AddWithValue("@PlayerYellowId", newGame.playerYellowId);
                sqlCommand.Parameters.AddWithValue("@CurrentTurn", newGame.CurrentTurn);
                sqlCommand.Parameters.AddWithValue("@Status", newGame.Status);

                try
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    string errormsg = e.Message;
                }
            }
        }

        public void UpdateGameStatus(GameDetails Game)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "UPDATE Games SET Status=@Status WHERE GameId=@Id";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Status", Game.Status);
                sqlCommand.Parameters.AddWithValue("@Id", Game.GameId);
                try
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    string errormsg = e.Message;
                }
            }
        }

        public char GetCurrentTurn(GameDetails Game)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "SELECT CurrentTurn FROM Games WHERE GameId=@Id";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", Game.GameId);
                try
                {
                    sqlConnection.Open();
                    var turn = sqlCommand.ExecuteScalar();
                    if (turn != null)
                    {
                        return Convert.ToChar(turn);
                    }
                    else
                    {
                        return ' ';
                    }
                }
                catch (Exception e)
                {
                    string errormsg = e.Message;
                    return ' ';
                }
            }
        }
public void UpdateCurrentTurn(GameDetails Game)
{
    using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
    {
        String sqlString = "UPDATE Games SET CurrentTurn=@CurrentTurn WHERE GameId=@Id";
        SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
        sqlCommand.Parameters.AddWithValue("@CurrentTurn", Game.CurrentTurn);
        sqlCommand.Parameters.AddWithValue("@Id", Game.GameId);
        try
        {
            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            string errormsg = e.Message;
        }
    }
}


        public char GetWinner(GameDetails Game)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "SELECT Winner FROM Games WHERE GameId=@Id";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", Game.GameId);
                try
                {
                    sqlConnection.Open();
                    var winner = sqlCommand.ExecuteScalar();
                    if (winner != null && winner != DBNull.Value)
                    {
                        return Convert.ToChar(winner);
                    }
                    else
                    {
                        return ' ';
                    }
                }
                catch (Exception e)
                {
                    string errormsg = e.Message;
                    return ' ';
                }
            }
        }

        public void UpdateWinner(GameDetails Game)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "UPDATE Games SET Winner=@Winner WHERE GameId=@Id";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Winner", Game.Winner);
                sqlCommand.Parameters.AddWithValue("@Id", Game.GameId);
                try
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    string errormsg = e.Message;
                }
            }
        }

        public void DeleteGame(GameDetails Game)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "DELETE FROM Games WHERE GameId=@Id";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", Game.GameId);
                try
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    string errormsg = e.Message;
                }
            }
        }
    }
}
