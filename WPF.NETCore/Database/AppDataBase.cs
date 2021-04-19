using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WPF.NETCore.Database
{

    public class AppDataBase : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options) 
            => options.UseSqlite("Data Source=P3teApp.db");


        public DbSet<DBLogError> LogErrors { get; set;}
        public DbSet<DBLogInfo> LogInfo { get; set;}
    }

    public class DBLog
    {
        public int Id { get; set; }
        public string Context { get; set; }
        public DateTime Date { get; set; }
    }

    public class DBLogError : DBLog
    {
    }

    public class DBLogInfo : DBLog
    {
    }
}
