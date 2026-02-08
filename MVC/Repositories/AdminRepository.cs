using MVC.interfaces;
using MVC.Models;
using Npgsql;

namespace MVC.implement;

public class AdminRepository : IAdminRepository
{
    private readonly NpgsqlConnection _conn;

    public AdminRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public List<t_Users> GetAllUsers()
    {
        var list = new List<t_Users>();

        try
        {
            if (_conn.State != System.Data.ConnectionState.Open)
                _conn.Open();

            string query = @"SELECT UserID, Name, Email, Gender, Status, ProfileImage, Role 
                                 FROM t_Users 
                                 WHERE Role = 'user' 
                                 ORDER BY UserID DESC;";

            using var cmd = new NpgsqlCommand(query, _conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new t_Users
                {
                    UserID = Convert.ToInt32(reader["UserID"]),
                    Name = reader["Name"]?.ToString(),
                    Email = reader["Email"]?.ToString(),
                    Gender = reader["Gender"]?.ToString(),
                    Status = reader["Status"]?.ToString(),
                    ProfileImage = reader["ProfileImage"]?.ToString(),
                    Role = reader["Role"]?.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(" GetAllUsers Error: " + ex.Message);
        }
        finally
        {
            if (_conn.State == System.Data.ConnectionState.Open)
                _conn.Close(); //
        }

        return list;
    }


    public List<t_transaction> GetTransactionsByUser(int userId)
    {
        var list = new List<t_transaction>();

        try
        {
            if (_conn.State != System.Data.ConnectionState.Open)
                _conn.Open();

            string query = @"SELECT transid, userid, category, amount, transdate, paymentmode, receiptimage, description
                         FROM t_transaction
                         WHERE userid = @userid
                         ORDER BY transdate DESC;";

            using var cmd = new NpgsqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@userid", userId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new t_transaction
                {
                    TransID = Convert.ToInt32(reader["transid"]),
                    UserID = Convert.ToInt32(reader["userid"]),
                    TransDate = reader["transdate"] != DBNull.Value ? Convert.ToDateTime(reader["transdate"]) : DateTime.Now,
                    Category = reader["category"]?.ToString() ?? "",
                    Amount = reader["amount"] != DBNull.Value ? Convert.ToSingle(reader["amount"]) : 0,
                    PaymentMode = reader["paymentmode"]?.ToString() ?? "",
                    ReceiptImage = reader["receiptimage"]?.ToString(),
                    Description = reader["description"]?.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("GetTransactionsByUser Error: " + ex.Message);
        }
        finally
        {
            if (_conn.State == System.Data.ConnectionState.Open)
                _conn.Close();
        }

        return list;
    }



    public bool ToggleStatus(int id)
    {
        try
        {
            if (_conn.State != System.Data.ConnectionState.Open)
                _conn.Open();

            string query = @"UPDATE t_Users
                                 SET Status = CASE WHEN Status = 'Active' THEN 'InActive' ELSE 'Active' END
                                 WHERE UserID = @id;";

            using var cmd = new NpgsqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ToggleStatus Error: " + ex.Message);
            return false;
        }
        finally
        {
            if (_conn.State == System.Data.ConnectionState.Open)
                _conn.Close();
        }
    }

}

