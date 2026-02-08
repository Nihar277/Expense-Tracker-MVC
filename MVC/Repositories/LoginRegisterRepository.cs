using MVC.interfaces;
using MVC.Models;
using Npgsql;

namespace MVC.implement;

public class LoginRegisterRepository : ILoginRegisterRepository
{
    private readonly NpgsqlConnection _conn;

    public LoginRegisterRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public t_Users Login(vm_Login user)
    {
        t_Users userData = new t_Users();
        try
        {
            var query = @"SELECT * FROM t_Users WHERE Email=@Email AND Password=@Password";
            using var cmd = new NpgsqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@Email", (object?)user.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Password", (object?)user.Password ?? DBNull.Value);

            _conn.Open();
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                userData.UserID = (int)reader["UserID"];
                userData.Name = (string)reader["Name"];
                userData.Email = (string)reader["Email"];
                userData.Gender = (string)reader["Gender"];
                userData.Status = (string)reader["Status"];
                userData.ProfileImage = (string)reader["ProfileImage"];
                userData.Role = (string)reader["Role"];
                _conn.Close();
            }
            else
            {
                _conn.Close();
                return null;
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("--->Login Implemetation Error: " + ex.Message);
        }
        finally
        {
            _conn.Close();
        }
        return userData;
    }

}