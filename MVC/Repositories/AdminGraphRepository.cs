using MVC.interfaces;
using MVC.Models;
using Npgsql;

namespace MVC.implement
{
    public class AdminGraphRepository : IAdminGraphRepository
    {
        private readonly NpgsqlConnection _conn;

        public AdminGraphRepository(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        // 1️⃣ Total active users
        // public int GetActiveUserCount()
        // {
        //     int count = 0;
        //     try
        //     {
        //         string qry = "SELECT COUNT(*) FROM t_Users WHERE Status = 'Active' AND role = 'user'";
        //         NpgsqlCommand cmd = new NpgsqlCommand(qry, _conn);


        //         _conn.Open();
        //         count = Convert.ToInt32(cmd.ExecuteScalar());
        //         _conn.Close();

        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("---> Total Active User Count Issue: " + ex.Message);
        //     }
        //     finally
        //     {
        //         _conn.Close();
        //     }
        //     return count;
        // }

        public int GetActiveUserCount()
        {
            int count = 0;
            try
            {
                using (var conn = new NpgsqlConnection(_conn.ConnectionString))
                {
                    conn.Open();
                    string qry = "SELECT COUNT(*) FROM t_Users WHERE Status = 'Active' AND role = 'user'";
                    using (var cmd = new NpgsqlCommand(qry, conn))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Total Active User Count Issue: " + ex.Message);
            }
            return count;
        }

        // 2️⃣ Category-wise expense
        public List<CategoryExpense> GetCategoryWiseExpense()
        {
            List<CategoryExpense> list = new List<CategoryExpense>();

            try
            {
                string qry = @"SELECT Category, SUM(Amount) AS TotalAmount 
                               FROM t_transaction 
                               GROUP BY Category";
                NpgsqlCommand cmd = new NpgsqlCommand(qry, _conn);

                _conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CategoryExpense
                        {
                            Category = dr["Category"].ToString(),
                            TotalAmount = Convert.ToDecimal(dr["TotalAmount"])
                        });
                    }
                }
                _conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Category Expense Error: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }

            return list;
        }

        // 3️⃣ Monthly expense trend
        public List<MonthlyExpense> GetMonthlyExpenseTrend()
        {
            List<MonthlyExpense> list = new List<MonthlyExpense>();

            try
            {
                string qry = @"
                    SELECT TO_CHAR(TransDate, 'Mon YYYY') AS MonthYear,
                           SUM(Amount) AS TotalAmount
                    FROM t_transaction
                    GROUP BY MonthYear
                    ORDER BY MIN(TransDate);";

                NpgsqlCommand cmd = new NpgsqlCommand(qry, _conn);
                _conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new MonthlyExpense
                        {
                            Month = dr["MonthYear"].ToString(),
                            TotalAmount = Convert.ToDecimal(dr["TotalAmount"])
                        });
                    }
                }
                _conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Monthly Expense Trend Error: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }

            return list;
        }

        // 4️⃣ Payment mode summary
        public List<PaymentModeExpense> GetPaymentModeSummary()
        {
            List<PaymentModeExpense> list = new List<PaymentModeExpense>();

            try
            {
                string qry = @"
                    SELECT PaymentMode, SUM(Amount) AS TotalAmount
                    FROM t_transaction
                    GROUP BY PaymentMode";

                NpgsqlCommand cmd = new NpgsqlCommand(qry, _conn);
                _conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new PaymentModeExpense
                        {
                            PaymentMode = dr["PaymentMode"].ToString(),
                            TotalAmount = Convert.ToDecimal(dr["TotalAmount"])
                        });
                    }
                }
                _conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Payment Mode Summary Error: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }

            return list;
        }

        // 5️⃣ Top 5 spending users
        public List<UserExpense> GetTopSpendingUsers()
        {
            List<UserExpense> list = new List<UserExpense>();

            try
            {
                string qry = @"
                    SELECT u.Name, SUM(t.Amount) AS TotalSpent
                    FROM t_transaction t
                    JOIN t_Users u ON t.UserID = u.UserID
                    GROUP BY u.Name
                    ORDER BY TotalSpent DESC
                    LIMIT 5";

                NpgsqlCommand cmd = new NpgsqlCommand(qry, _conn);
                _conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new UserExpense
                        {
                            Name = dr["Name"].ToString(),
                            TotalSpent = Convert.ToDecimal(dr["TotalSpent"])
                        });
                    }
                }
                _conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Top Spending Users Error: " + ex.Message);
            }
            finally
            {
                _conn.Close();
            }

            return list;
        }

        public int GetInactiveUserCount()
        {
            int count = 0;
            try
            {
                using (var conn = new NpgsqlConnection(_conn.ConnectionString))
                {
                    conn.Open();
                    string qry = "SELECT COUNT(*) FROM t_Users WHERE Status = 'InActive' AND role = 'user'";
                    using (var cmd = new NpgsqlCommand(qry, conn))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Inactive User Count Issue: " + ex.Message);
            }
            return count;
        }

        public int GetTotalUserCount()
        {
            int count = 0;
            try
            {
                using (var conn = new NpgsqlConnection(_conn.ConnectionString))
                {
                    conn.Open();
                    string qry = "SELECT COUNT(*) FROM t_Users WHERE role = 'user'";
                    using (var cmd = new NpgsqlCommand(qry, conn))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("---> Total User Count Issue: " + ex.Message);
            }
            return count;
        }

    }
}