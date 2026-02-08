using System.ComponentModel;
using System.ComponentModel;
using MVC.interfaces;
using MVC.Models;
using Npgsql;

namespace MVC.implement;

public class ExpenseRepository : IExpenseRepository
{
    private readonly string _conn;

    public ExpenseRepository(IConfiguration configuration)
    {
        _conn = configuration.GetConnectionString("pgconn");
    }

    public async Task<int> deleteExpense(int id)
    {
        
        var q = "DELETE FROM t_transaction WHERE transid=@id";

        try
        {
            using var conn = new NpgsqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();

            return 1;
        }
        catch (Exception e)
        {
            Console.WriteLine("Getting Error While Deleting: " + e.Message);
            return 0;
        }
    }

    public async Task<List<t_transaction>> getAllExpense(int userid)
    {
        var transactions = new List<t_transaction>();
        var q = @"SELECT transid, userid, category, amount, transdate, paymentmode, receiptimage, description 
                  FROM t_transaction WHERE userid = @userid ORDER BY transid ASC";

        try
        {
            using var conn = new NpgsqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@userid", userid);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                transactions.Add(new t_transaction()
                {
                    TransID = reader.GetInt32(0),
                    UserID = reader.GetInt32(1),
                    Category = reader.GetString(2),
                    Amount = reader.GetFloat(3),
                    TransDate = reader.GetDateTime(4),
                    PaymentMode = reader.GetString(5),
                    ReceiptImage = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Description = reader.IsDBNull(7) ? null : reader.GetString(7)
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error fetching expenses: " + e.Message);
        }

        return transactions;
    }

    public async Task<int> addExpense(t_transaction trans)
    {
        var q = @"INSERT INTO t_transaction 
                  (userid, category, amount, transdate, paymentmode, receiptimage, description)
                  VALUES (@userid, @category, @amount, @transdate, @paymentmode, @receiptimage, @description)";

        try
        {
            using var conn = new NpgsqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(q, conn);
            // Replace with real logic as needed

            cmd.Parameters.AddWithValue("@userid", trans.UserID);
            cmd.Parameters.AddWithValue("@category", trans.Category ?? "");
            cmd.Parameters.AddWithValue("@amount", trans.Amount);
            cmd.Parameters.AddWithValue("@transdate", trans.TransDate);
            cmd.Parameters.AddWithValue("@paymentmode", trans.PaymentMode ?? "");
            cmd.Parameters.AddWithValue("@receiptimage", (object?)trans.ReceiptImage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)trans.Description ?? DBNull.Value);

            int result = await cmd.ExecuteNonQueryAsync();
            return result > 0 ? 1 : 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error While Adding Expense : " + e.Message);
            return 0;
        }
    }

    public async Task<t_transaction> getExpenseById(int id)
    {
        var q = @"SELECT transid, userid, category, amount, transdate, paymentmode, receiptimage, description
                  FROM t_transaction WHERE transid = @id";

        t_transaction? trans = null;

        try
        {
            using var conn = new NpgsqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                trans = new t_transaction()
                {
                    TransID = reader.GetInt32(0),
                    UserID = reader.GetInt32(1),
                    Category = reader.GetString(2),
                    Amount = reader.GetFloat(3),
                    TransDate = reader.GetDateTime(4),
                    PaymentMode = reader.GetString(5),
                    ReceiptImage = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Description = reader.IsDBNull(7) ? null : reader.GetString(7)
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching expense by ID: " + ex.Message);
        }

        return trans!;
    }

    public async Task<int> updateExpense(t_transaction transaction)
    {
        var q = @"UPDATE t_transaction 
                  SET category = @category, amount = @amount, transdate = @transdate,
                      paymentmode = @paymentmode, receiptimage = @receiptimage, description = @description
                  WHERE transid = @transid";

        try
        {
            using var conn = new NpgsqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(q, conn);

            cmd.Parameters.AddWithValue("@category", transaction.Category ?? "");
            cmd.Parameters.AddWithValue("@amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@transdate", transaction.TransDate);
            cmd.Parameters.AddWithValue("@paymentmode", transaction.PaymentMode ?? "");
            cmd.Parameters.AddWithValue("@receiptimage", (object?)transaction.ReceiptImage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)transaction.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@transid", transaction.TransID);

            int result = await cmd.ExecuteNonQueryAsync();
            return result > 0 ? 1 : 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating expense: " + ex.Message);
            return 0;
        }
    }

    public async Task<int> totalExpense(int id)
    {
        int totalExpense = 0;
        var q = "SELECT SUM(amount) FROM t_transaction WHERE userid = @userid";

        try
        {
            using var conn = new NpgsqlConnection(_conn);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(q, conn);
            cmd.Parameters.AddWithValue("@userid", id);

            var read = await cmd.ExecuteScalarAsync();
            totalExpense = read != DBNull.Value ? Convert.ToInt32(read) : 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error While Fetching Total Expense: " + e.Message);
        }

        return totalExpense;
    }
}