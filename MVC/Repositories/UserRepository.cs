using System.ComponentModel;
using System.ComponentModel;
using MVC.interfaces;
using MVC.Models;
using Npgsql;

namespace MVC.implement;

public class UserRepository : IUserRepository
{
    private readonly NpgsqlConnection _conn;

    public UserRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> Register(t_Users user)
    {
        int status = 0;

        try
        {
            await _conn.CloseAsync();
            string query = "SELECT * FROM t_users WHERE email = @email;";
            using NpgsqlCommand cmd = new NpgsqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("email", user.Email);

            await _conn.OpenAsync();
            using NpgsqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                await _conn.CloseAsync();
                return 0;
            }
            else
            {
                await _conn.CloseAsync();
                string insertQuery = @"insert into t_users (name, email, password, gender, status, profileimage, role)
			                                        values (@name, @email, @password, @gender, @status, @image, @role);";

                using NpgsqlCommand cmd1 = new NpgsqlCommand(insertQuery, _conn);
                cmd1.Parameters.AddWithValue("name", user.Name);
                cmd1.Parameters.AddWithValue("email", user.Email);
                cmd1.Parameters.AddWithValue("password", user.Password);
                cmd1.Parameters.AddWithValue("gender", user.Gender);
                cmd1.Parameters.AddWithValue("status", user.Status);
                cmd1.Parameters.AddWithValue("image", user.ProfileImage);
                cmd1.Parameters.AddWithValue("role", user.Role);
                await _conn.OpenAsync();
                await cmd1.ExecuteNonQueryAsync();
                await _conn.CloseAsync();
                return 1;
            }
        }
        catch (Exception ex)
        {
            await _conn.CloseAsync();
            Console.WriteLine("Register Faild, Error :- " + ex.Message);
            return -1;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    //Change Password 
    public bool ChangePassword(int id, string currentPassword, string newPassword)
    {
        bool isChanged = false;

        try
        {
            _conn.Open();
            //verify current password
            string checkQuery = "SELECT COUNT(*) FROM t_users WHERE userid = @id AND password=@current";
            using (var cmd = new NpgsqlCommand(checkQuery, _conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@current", currentPassword);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if(count == 0)
                {
                    return false;
                }

            }

            //update password
            string updateQuery = "UPDATE t_users SET password = @new WHERE userid = @id";
            using (var ucmd = new NpgsqlCommand(updateQuery, _conn))
            {
                ucmd.Parameters.AddWithValue("@new", newPassword);
                ucmd.Parameters.AddWithValue("@id", id);

                int rows = ucmd.ExecuteNonQuery();

                isChanged = rows > 0;
            }
            
        }
        catch (Exception ex)
        {
            
            isChanged = false;
        }

        finally
        {
            if(_conn.State == System.Data.ConnectionState.Open)
            {
                _conn.Close();
            }
        }

        return isChanged;
    }

    //update profile

    //get by id
    public t_Users GetById(int id)
{
    t_Users user = null;

    try
    {
        if (id <= 0)
            return null;

        string query = "SELECT userid, name, email, password, gender, status, profileimage, role FROM t_users WHERE userid = @id";

        _conn.Open();
        using (var cmd = new NpgsqlCommand(query, _conn))
        {
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new t_Users
                        {
                            UserID = reader.GetInt32(reader.GetOrdinal("userid")),
                            Name = reader["name"]?.ToString(),
                            Email = reader["email"]?.ToString(),
                            Password = reader["password"]?.ToString(),
                            Gender = reader["gender"]?.ToString(),
                            Status = reader["status"]?.ToString(),
                            ProfileImage = reader["profileimage"]?.ToString(),
                            Role = reader["role"]?.ToString()
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // optional: log the error
            user = null;
        }
        finally
        {
            if (_conn.State == System.Data.ConnectionState.Open)
                _conn.Close();
        }

        return user;
    }

    //profile update
    public bool UpdateUser(t_Users user)
    {
        bool isUpdated = false;
        try
        {
            _conn.Open();
            string updateQuery = @"UPDATE t_Users SET 
                                Name = @name,
                                Email = @email,
                                Gender = @gender,
                                Status = @status,
                                ProfileImage = @profileimage,
                                Role = @role WHERE UserID = @userid";
            
            using (var cmd =  new NpgsqlCommand(updateQuery, _conn))
            {
                cmd.Parameters.AddWithValue("@name", user.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@gender", user.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@status", user.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@role", user.Role ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@profileImage", user.ProfileImage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@userid", user.UserID);

                int rows = cmd.ExecuteNonQuery();
                isUpdated = rows > 0;
            }
        }
        catch (Exception ex)
        {
            isUpdated = false;
        }
        finally
        {
            if (_conn.State == System.Data.ConnectionState.Open)
            {
                _conn.Close();
            }
        }

        return isUpdated;
    }
}