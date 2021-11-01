﻿using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TPaper.Delivery
{
    public class DeliveryContextFactory : IDesignTimeDbContextFactory<DeliveryDbContext>
    {
        public DeliveryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DeliveryDbContext>();

            string sqlString = Environment.GetEnvironmentVariable("SqlPaperString");
            string sqlPassword = Environment.GetEnvironmentVariable("SqlPaperPassword");
            string connectionString = new SqlConnectionStringBuilder(sqlString) { Password = sqlPassword }.ConnectionString;
            
            optionsBuilder.UseSqlServer(connectionString);

            return new DeliveryDbContext(optionsBuilder.Options);
        }
    }
}