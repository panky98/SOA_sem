using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataMicroservice.Models
{
    public class StreamCreation
    {
        private string sql;

        public string Sql { get => sql; set => sql = value; }

        public StreamCreation(string s)
        {
            this.Sql = s;
        }
    }
}
