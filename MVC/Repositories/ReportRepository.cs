using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MVC.interfaces;
using MVC.Models;

namespace MVC.implement
{
    public class ReportRepository : IReportRepository
    {
        private readonly string _conn;

        public ReportRepository(IConfiguration config)
        {
            _conn = config.GetConnectionString("pgconn")!;
        }

        
        public List<dynamic> FetchAggregatedReport(DateTime startDate, DateTime endDate)
        {
            var reportData = new List<dynamic>();

            using (var conn = new NpgsqlConnection(_conn))
            {
                conn.Open();
                string query = @"
                    SELECT t.userid, u.name AS username, t.category, t.amount, t.paymentmode, t.transdate
                    FROM t_transaction t
                    JOIN t_users u ON t.userid = u.userid
                    WHERE t.transdate >= @start AND t.transdate <= @end
                    ORDER BY t.transdate DESC;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("start", NpgsqlTypes.NpgsqlDbType.Date, startDate);
                    cmd.Parameters.AddWithValue("end", NpgsqlTypes.NpgsqlDbType.Date, endDate);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportData.Add(new
                            {
                                userId = reader["userid"].ToString(),
                                userName = reader["username"].ToString(),
                                category = reader["category"].ToString(),
                                amount = Convert.ToDecimal(reader["amount"]),
                                paymentMode = reader["paymentmode"].ToString(),
                                transDate = Convert.ToDateTime(reader["transdate"]).ToString("yyyy-MM-dd")
                            });
                        }
                    }
                }
            }

            return reportData;
        }

        
        public (List<dynamic> transactions, decimal grandTotal) FetchTransactionsForPdf(DateTime startDate, DateTime endDate)
        {
            var transactions = new List<dynamic>();
            decimal grandTotal = 0;

            using (var conn = new NpgsqlConnection(_conn))
            {
                conn.Open();
                string query = @"
                    SELECT t.userid, u.name AS username, t.category, t.amount, t.paymentmode, t.transdate
                    FROM t_transaction t
                    JOIN t_users u ON t.userid = u.userid
                    WHERE t.transdate >= @start AND t.transdate <= @end
                    ORDER BY t.transdate DESC;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("start", NpgsqlTypes.NpgsqlDbType.Date, startDate);
                    cmd.Parameters.AddWithValue("end", NpgsqlTypes.NpgsqlDbType.Date, endDate);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal amount = Convert.ToDecimal(reader["amount"]);
                            grandTotal += amount;

                            transactions.Add(new
                            {
                                userId = reader["userid"].ToString(),
                                userName = reader["username"].ToString(),
                                category = reader["category"].ToString(),
                                amount = amount,
                                paymentMode = reader["paymentmode"].ToString(),
                                transDate = Convert.ToDateTime(reader["transdate"]).ToString("yyyy-MM-dd")
                            });
                        }
                    }
                }
            }

            return (transactions, grandTotal);
        }
    }
}
