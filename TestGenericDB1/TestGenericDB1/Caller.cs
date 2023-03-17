using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace TestGenericDB1
{



    internal class Caller
    {
        public Caller() { }
        public void Call() {

            Notes obj = new Notes();
            obj.CATEGORY = "First Category";
            obj.id = 1;

            string con = "Data Source=(localdb)\\MSSQLLocalDB;" +
                          "Initial Catalog=DelDB;" +
                          "User id=user2;" +
                          "Password=Aug2021!Aug2021!;";

            Entity< Notes> entity = new Entity<Notes>();
            entity.fileOrServerOrConnection = con;
            entity.Add(obj);

            // Code to get the object values
            //Entity<NOTES> entity = new Entity<NOTES>();
            //NOTES note = entity.Get(typeof(System.Int64), "ID", 1);


        }
    }

    [Table(Name = "dbo.notes")]
    public class Notes
    {
        [Column (IsPrimaryKey =true)]
        public int id = 0;
        [Column]
        public string CATEGORY ;
    }


}
