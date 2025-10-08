using System.Data;
using Microsoft.Data.SqlClient;

namespace FyraIRad.Models
{
    public class UserMethods
    {
        public UserMethods() { }

        public List<UserDetails> GetUserDetails(out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Project4inaRow;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            String sqlString = "SELECT * FROM Users";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataSet dataSet = new DataSet();

            List<UserDetails> userList = new List<UserDetails>();

            try
            {
                sqlConnection.Open();
                sqlDataAdapter.Fill(dataSet, "UserList");

                int i = 0;
                int count = 0;

                count = dataSet.Tables["UserList"].Rows.Count;

                if (i < count)
                {
                    while (i < count)
                    {
                        UserDetails user = new UserDetails();
                        user.Id = Convert.ToInt32(dataSet.Tables["UserList"].Rows[i]["UserId"]);
                        user.Username = Convert.ToString(dataSet.Tables["UserList"].Rows[i]["Username"]);
                        user.Password = Convert.ToString(dataSet.Tables["UserList"].Rows[i]["Password"]);

                        i++;
                        userList.Add(user);
                    }
                    errormsg = "";
                    return userList;
                }
                else
                {
                    errormsg = "No users found in database";
                    return userList;

                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return userList;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public UserDetails GetOneUserDetails(int id)
        {
            return new UserDetails();
        }


        public void CreateUser(UserDetails newUser)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Project4inaRow;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            String sqlString = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
            SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Username", newUser.Username);
            sqlCommand.Parameters.AddWithValue("@Password", newUser.Password);
            try
            {
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string errormsg = e.Message;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
