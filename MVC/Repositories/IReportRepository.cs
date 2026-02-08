using MVC.Models;
namespace MVC.interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MVC.interfaces;


public interface IReportRepository
{
    public List<dynamic> FetchAggregatedReport(DateTime startDate, DateTime endDate);

    public (List<dynamic> transactions, decimal grandTotal) FetchTransactionsForPdf(DateTime startDate, DateTime endDate);
    
}