using System.Data;
using Microsoft.Data.SqlClient;

namespace FyraIRad.Models
{
    public class UserMethods
    {

        private readonly string _connectionString;


        public UserMethods(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<UserDetails> GetUserDetails(out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
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
            }

          
        }


        public void CreateUser(UserDetails newUser)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
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
            }
        }

        public UserDetails GetOneUserDetails(int id, out string errormsg)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "SELECT * FROM Users WHERE UserId=@Id";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataSet dataSet = new DataSet();

                UserDetails user = new UserDetails();

                sqlCommand.Parameters.AddWithValue("@Id", id);

                try
                {
                    sqlConnection.Open();
                    sqlDataAdapter.Fill(dataSet, "UserList");

                    int i = 0;
                    int count = 0;

                    count = dataSet.Tables["UserList"].Rows.Count;

                    if (i < count)
                    {

                        user.Id = Convert.ToInt32(dataSet.Tables["UserList"].Rows[i]["UserId"]);
                        user.Username = Convert.ToString(dataSet.Tables["UserList"].Rows[i]["Username"]);
                        user.Password = Convert.ToString(dataSet.Tables["UserList"].Rows[i]["Password"]);


                        errormsg = "";
                        return user;
                    }
                    else
                    {
                        errormsg = "No users found in database";
                        return user;

                    }
                }
                catch (Exception e)
                {
                    errormsg = e.Message;
                    return user;
                }
               
            }
        }

        public void EditUser(UserDetails editUser)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                String sqlString = "UPDATE Users SET Username=@Username, Password=@Password WHERE UserId=@Id";
                SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Id", editUser.Id);
                sqlCommand.Parameters.AddWithValue("@Username", editUser.Username);
                sqlCommand.Parameters.AddWithValue("@Password", editUser.Password);
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
}
