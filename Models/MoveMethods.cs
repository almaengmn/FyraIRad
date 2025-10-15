using Microsoft.Data.SqlClient;
using System.Data;

namespace FyraIRad.Models
{
    public class MoveMethods
    {
        private readonly string _connectionString;


        public MoveMethods(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<MoveDetails> GetMoveDetailsForGame(int GameId, out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "SELECT * FROM Moves WHERE GameId=@GameId";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlCommand.Parameters.AddWithValue("@GameId", GameId);
                DataSet dataSet = new DataSet();
                List<MoveDetails> MoveList = new List<MoveDetails>();
                try
                {
                    sqlConnection.Open();
                    sqlDataAdapter.Fill(dataSet, "MoveList");
                    int i = 0;
                    int count = 0;
                    count = dataSet.Tables["MoveList"].Rows.Count;
                    if (i < count)
                    {
                        while (i < count)
                        {
                            MoveDetails Move = new MoveDetails();
                            Move.MoveId = Convert.ToInt32(dataSet.Tables["MoveList"].Rows[i]["MoveId"]);
                            Move.GameId = Convert.ToInt32(dataSet.Tables["MoveList"].Rows[i]["GameId"]);
                            Move.PlayerId = Convert.ToInt32(dataSet.Tables["MoveList"].Rows[i]["PlayerId"]);
                            Move.ColumnIndex = Convert.ToInt32(dataSet.Tables["MoveList"].Rows[i]["ColumnIndex"]);
                            Move.RowIndex = Convert.ToInt32(dataSet.Tables["MoveList"].Rows[i]["RowIndex"]);
                            Move.DiscColor = Convert.ToChar(dataSet.Tables["MoveList"].Rows[i]["DiscColor"]);
                            Move.MoveTime = Convert.ToDateTime(dataSet.Tables["MoveList"].Rows[i]["MoveTime"]);
                            i++;
                            MoveList.Add(Move);
                        }
                        errormsg = "";
                        return MoveList;
                    }
                    else
                    {
                        errormsg = "No Moves found in database";
                        return MoveList;
                    }
                }
                catch (Exception ex)
                {
                    errormsg = ex.Message;
                    return MoveList;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public void CreateMove(MoveDetails newMove)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "INSERT INTO Moves (GameId, PlayerId, ColumnIndex, RowIndex, DiscColor, MoveTime) VALUES (@GameId, @PlayerId, @Column, @Row, @Color, GETDATE())";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@GameId", newMove.GameId);
                sqlCommand.Parameters.AddWithValue("@PlayerId", newMove.PlayerId);
                sqlCommand.Parameters.AddWithValue("@Column", newMove.ColumnIndex);
                sqlCommand.Parameters.AddWithValue("@Row", newMove.RowIndex);
                sqlCommand.Parameters.AddWithValue("@Color", newMove.DiscColor);
                
                try
                {
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
    }
}
