using System;
using System.Linq;
using System.Threading.Tasks;
using FastTests;
using FastTests.Server.Documents.Queries.Parser;
using FastTests.Voron.Backups;
using FastTests.Voron.Compaction;
using RachisTests.DatabaseCluster;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries;
using Raven.Tests.Core.Utils.Entities;
using SlowTests.Authentication;
using SlowTests.Bugs.MapRedue;
using SlowTests.Client;
using SlowTests.Client.Attachments;
using SlowTests.Issues;
using SlowTests.MailingList;
using Sparrow.Logging;
using StressTests.Client.Attachments;
using StressTests.Voron.Issues;
using Xunit;

namespace Tryouts
{
    public class User
    {
        public string Id;
        public string FirstName;
        public string LastName;
        public string Address;
        public int Count;
        public int Age;
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            //for (int i = 0; i < 1000; i++)
            //{
            //    Console.WriteLine(i);
            //    using (var test = new RDBC_128())
            //    {
            //        test.IndexingOfLoadDocumentWhileChanged();
            //    }
            //}

            var store = new DocumentStore
            {
                Urls = new string[] {"http://127.0.0.1:8081"},
                Database = "Test"
            };
            store.Initialize();

            using (var session = store.OpenSession())
            {

                //var u = session.Load<User>("users/3");
                //var metadata = session.Advanced.GetMetadataFor(u);
                //metadata.Add("Key","Value");

                //session.SaveChanges();

                session.Advanced.Exists("users/2");
            }
        }
    }
}
