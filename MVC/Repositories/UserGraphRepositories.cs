using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MVC.Models;
using Npgsql;

namespace MVC.Repositories
{
    public class UserGraphRepositories : IUserGraphRepositories
    {
        private readonly NpgsqlConnection _conn;
        public UserGraphRepositories(NpgsqlConnection npgsqlConnection)
        {
            _conn = npgsqlConnection;
        }

        public List<vm_UserTransactionGraph> getAllTransactionByUserId(int id)
        {
            List<vm_UserTransactionGraph> userGraphs = new List<vm_UserTransactionGraph>();
            try
            {
                _conn.Open();
                var stringQuery = "select t.category, sum(t.amount) as amount from t_users u join t_transaction t on u.userid=t.userid where t.userid=@userid group by t.category;";
                NpgsqlCommand cmd = new NpgsqlCommand(stringQuery, _conn);
                cmd.Parameters.AddWithValue("userid", id);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var trans = new vm_UserTransactionGraph
                    {
                        Category = Convert.ToString(reader["category"]),
                        Amount = Convert.ToInt32(reader["amount"]),
                    };
                    userGraphs.Add(trans);
                }
                // return userGraphs;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }
            return userGraphs;
        }

        public List<vm_UserBarChart> getAllTransactionByMonth(int id)
        {
            List<vm_UserBarChart> userGraphs = new List<vm_UserBarChart>();
            try
            {
                _conn.Open();
                var stringQuery = @"with c as(
                    SELECT *,EXTRACT(MONTH FROM transdate) as month from t_transaction
                    )select sum(amount) as amount,month  from c where userid = @userid group by month;";
                NpgsqlCommand cmd = new NpgsqlCommand(stringQuery, _conn);
                cmd.Parameters.AddWithValue("userid", id);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(reader["month"]));
                    var trans = new vm_UserBarChart
                    {
                        Month = monthName,
                        Amount = Convert.ToInt32(reader["amount"]),
                    };
                    userGraphs.Add(trans);
                }
                // return userGraphs;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }
            return userGraphs;
        }
    }
}