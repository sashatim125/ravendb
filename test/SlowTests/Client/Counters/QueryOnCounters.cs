﻿using System.Collections.Generic;
using System.Linq;
using FastTests;
using Raven.Client.Documents.Queries;
using Xunit;

namespace SlowTests.Client.Counters
{
    public class QueryOnCounters : RavenTestBase
    {
        [Fact]
        public void RawQuerySelectSingleCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult>("from users select counter(\"Downloads\")")
                        .ToList();

                    Assert.Equal(3, query.Count);
                    Assert.Equal(100, query[0].Downloads);
                    Assert.Equal(200, query[1].Downloads);
                    Assert.Null(query[2].Downloads);
                }
            }
        }

        [Fact]
        public void RawQuerySelectSingleCounterWithDocAlias()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult>("from users as u select counter(u, \"Downloads\")")
                        .ToList();

                    Assert.Equal(3, query.Count);
                    Assert.Equal(100, query[0].Downloads);
                    Assert.Equal(200, query[1].Downloads);
                    Assert.Null(query[2].Downloads);
                }
            }
        }

        [Fact]
        public void RawQuerySelectSingleCounterWithCounterAlias()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult2>("from users select counter(\"downloads\") as DownloadsCount")
                        .ToList();

                    Assert.Equal(3, query.Count);
                    Assert.Equal(100, query[0].DownloadsCount);
                    Assert.Equal(200, query[1].DownloadsCount);
                    Assert.Null(query[2].DownloadsCount);
                }
            }
        }

        [Fact]
        public void RawQuerySelectSingleCounterRawValues()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult3>("from users as u select counterRaw(u, \"downloads\")")
                        .ToList();

                    Assert.Equal(3, query.Count);

                    Assert.Equal(100, query[0].Downloads.Values.Single());
                    Assert.Equal(200, query[1].Downloads.Values.Single());
                    Assert.Equal(0, query[2].Downloads.Count);
                }
            }
        }

        [Fact]
        public void RawQuerySelectMultipuleCounters()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/1-A", "Likes", 200);

                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 400);
                    session.Advanced.Counters.Increment("users/2-A", "Likes", 800);

                    session.Advanced.Counters.Increment("users/3-A", "Likes", 1600);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult>("from users select counter(\"Downloads\"), counter(\"Likes\")")
                        .ToList();

                    Assert.Equal(3, query.Count);
                    Assert.Equal(100, query[0].Downloads);
                    Assert.Equal(200, query[0].Likes);

                    Assert.Equal(400, query[1].Downloads);
                    Assert.Equal(800, query[1].Likes);

                    Assert.Null(query[2].Downloads);
                    Assert.Equal(1600, query[2].Likes);
                }
            }
        }

        [Fact]
        public void RawQuerySimpleProjectionWithCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);

                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);

                    session.Advanced.Counters.Increment("users/3-A", "Likes", 400);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult>("from users select Name, counter(\"Downloads\")")
                        .ToList();

                    Assert.Equal(3, query.Count);

                    Assert.Equal("Jerry", query[0].Name);
                    Assert.Equal(100, query[0].Downloads);

                    Assert.Equal("Bob", query[1].Name);
                    Assert.Equal(200, query[1].Downloads);

                    Assert.Equal("Pigpen", query[2].Name);
                    Assert.Null(query[2].Downloads);
                }
            }
        }

        [Fact]
        public void RawQueryJsProjectionWithCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "likes", 400);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult>("from users as u select { Name : u.Name, Downloads : counter(u, \"downloads\") }")
                        .ToList();

                    Assert.Equal(3, query.Count);

                    Assert.Equal("Jerry", query[0].Name);
                    Assert.Equal(100, query[0].Downloads);

                    Assert.Equal("Bob", query[1].Name);
                    Assert.Equal(200, query[1].Downloads);

                    Assert.Equal("Pigpen", query[2].Name);
                    Assert.Null(query[2].Downloads);
                }
            }
        }

        [Fact]
        public void RawQueryJsProjectionWithCounterRawValues()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "downloads", 100);
                    session.Advanced.Counters.Increment("users/1-A", "likes", 200);

                    session.Advanced.Counters.Increment("users/2-A", "downloads", 300);
                    session.Advanced.Counters.Increment("users/2-A", "likes", 400);

                    session.Advanced.Counters.Increment("users/3-A", "likes", 500);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult4>("from users as u select { Name : u.Name, Downloads : counter(u, \"downloads\"), Likes : counterRaw(u, \"likes\") }")
                        .ToList();

                    Assert.Equal(3, query.Count); 

                    Assert.Equal("Jerry", query[0].Name);
                    Assert.Equal(100, query[0].Downloads);
                    Assert.Equal(200, query[0].Likes.Values.Single());

                    Assert.Equal("Bob", query[1].Name);
                    Assert.Equal(300, query[1].Downloads);
                    Assert.Equal(400, query[1].Likes.Values.Single());

                    Assert.Equal("Pigpen", query[2].Name);
                    Assert.Equal(500, query[2].Likes.Values.Single());
                    Assert.Null(query[2].Downloads);
                }
            }
        }

        [Fact]
        public void RawQuerySimpleProjectionWithParameters()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/1-A", "Likes", 200);

                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 300);
                    session.Advanced.Counters.Increment("users/2-A", "Likes", 400);

                    session.Advanced.Counters.Increment("users/3-A", "Downloads", 500);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult>("from users select counter($p0), counter($p1)")
                        .AddParameter("p0", "Downloads")
                        .AddParameter("p1", "Likes")
                        .ToList();

                    Assert.Equal(3, query.Count);
                    Assert.Equal(100, query[0].Downloads);
                    Assert.Equal(200, query[0].Likes);

                    Assert.Equal(300, query[1].Downloads);
                    Assert.Equal(400, query[1].Likes);

                    Assert.Equal(500, query[2].Downloads);
                    Assert.Null(query[2].Likes);
                }
            }
        }

        [Fact]
        public void RawQueryJsProjectionWithParameters()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/1-A", "Likes", 200);

                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 300);
                    session.Advanced.Counters.Increment("users/2-A", "Likes", 400);

                    session.Advanced.Counters.Increment("users/3-A", "Downloads", 500);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult2>("from users as u select { DownloadsCount: counter(u, $p0), LikesCount: counter(u, $p1) }")
                        .AddParameter("p0", "Downloads")
                        .AddParameter("p1", "Likes")
                        .ToList();

                    Assert.Equal(3, query.Count);
                    Assert.Equal(100, query[0].DownloadsCount);
                    Assert.Equal(200, query[0].LikesCount);

                    Assert.Equal(300, query[1].DownloadsCount);
                    Assert.Equal(400, query[1].LikesCount);

                    Assert.Equal(500, query[2].DownloadsCount);
                    Assert.Null(query[2].LikesCount);
                }
            }
        }

        [Fact]
        public void RawQuerySelectCounterWithParameterAndAlias()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "downloads", 300);
                    session.Advanced.Counters.Increment("users/3-A", "downloads", 500);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult2>("from users select counter($p0) as DownloadsCount")
                        .AddParameter("p0", "downloads")
                        .ToList();

                    Assert.Equal(3, query.Count);
                    Assert.Equal(100, query[0].DownloadsCount);
                    Assert.Equal(300, query[1].DownloadsCount);
                    Assert.Equal(500, query[2].DownloadsCount);
                }
            }
        }

        [Fact]
        public void RawQuerySelectCounterWithWhere()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry", Age = 75 }, "users/1-A");
                    session.Store(new User { Name = "Bob", Age = 68 }, "users/2-A");
                    session.Store(new User { Name = "Pigpen", Age = 71 }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult>("from users where Age > 70 select Name, counter(\"Downloads\")")
                        .ToList();

                    Assert.Equal(2, query.Count);

                    Assert.Equal("Jerry", query[0].Name);
                    Assert.Equal(100, query[0].Downloads);

                    Assert.Equal("Pigpen", query[1].Name);
                    Assert.Null(query[1].Downloads);

                }
            }
        }

        [Fact]
        public void RawQuerySelectCounterFromLoadedDoc()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry", FriendId = "users/2-A" }, "users/1-A");
                    session.Store(new User { Name = "Bob", FriendId = "users/3-A" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen", FriendId = "users/1-A" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult7>(@"from users as u 
                                                    load u.FriendId as f 
                                                    select Name,
                                                           counter(u, $p0),
                                                           counter(f, $p0) as FriendsDownloads")
                        .AddParameter("p0", "Downloads")
                        .ToList();

                    Assert.Equal(3, query.Count);

                    Assert.Equal("Jerry", query[0].Name);
                    Assert.Equal(100, query[0].Downloads);
                    Assert.Equal(200, query[0].FriendsDownloads);

                    Assert.Equal("Bob", query[1].Name);
                    Assert.Equal(200, query[1].Downloads);
                    Assert.Null(query[1].FriendsDownloads);

                    Assert.Equal("Pigpen", query[2].Name);
                    Assert.Null(query[2].Downloads);
                    Assert.Equal(100, query[2].FriendsDownloads);

                }
            }
        }

        [Fact]
        public void RawQuerySelectCounterFromLoadedDocJsProjection()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry", FriendId = "users/2-A"}, "users/1-A");
                    session.Store(new User { Name = "Bob", FriendId = "users/3-A"}, "users/2-A");
                    session.Store(new User { Name = "Pigpen", FriendId = "users/1-A"}, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult7>(@"from users as u 
                                                 load u.FriendId as f 
                                                 select  
                                                 { 
                                                    Name: u.Name, 
                                                    Downloads : counter(u, $p0), 
                                                    FriendsDownloads : counter(f, $p0)  
                                                 }")
                        .AddParameter("p0", "Downloads")
                        .ToList();

                    Assert.Equal(3, query.Count);

                    Assert.Equal("Jerry", query[0].Name);
                    Assert.Equal(100, query[0].Downloads);
                    Assert.Equal(200, query[0].FriendsDownloads);

                    Assert.Equal("Bob", query[1].Name);
                    Assert.Equal(200, query[1].Downloads);
                    Assert.Null(query[1].FriendsDownloads);

                    Assert.Equal("Pigpen", query[2].Name);
                    Assert.Null(query[2].Downloads);
                    Assert.Equal(100, query[2].FriendsDownloads);

                }
            }
        }

        [Fact]
        public void RawQueryGetCounterValueFromMetadata()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Likes", 500);
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult6>(@"
                            from Users as u
                            select
                            {
                                Counter: counter(u, u[""@metadata""][""@counters""][0])
                            }")
                        .ToList();

                    Assert.Equal(2, query.Count);
                    Assert.Equal(100, query[0].Counter);
                    Assert.Equal(500, query[1].Counter);

                }
            }
        }

        [Fact]
        public void RawQueryGetAllCountersValuesFromMetadata()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/1-A", "Likes", 200);

                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 300);
                    session.Advanced.Counters.Increment("users/2-A", "Likes", 400);

                    session.Advanced.Counters.Increment("users/3-A", "Downloads", 500);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Advanced
                        .RawQuery<CounterResult5>(@"
                            declare function getCounters(doc)
                            {
                                var names = doc[""@metadata""][""@counters""];
                                if (names == null)
                                    return names;
                                var res = { };
                                var length = names.length;
                                for (var i = 0; i < length; i++)
                                {
                                    var curName = names[i];
                                    res[curName] = counter(doc, curName);
                                }
                                return res;
                            }
                            from Users as u
                            select
                            {
                                Counters: getCounters(u)
                            }")
                        .ToList();

                    Assert.Equal(3, query.Count);

                    Assert.Equal(100, query[0].Counters["Downloads"]);
                    Assert.Equal(200, query[0].Counters["Likes"]);

                    Assert.Equal(300, query[1].Counters["Downloads"]);
                    Assert.Equal(400, query[1].Counters["Likes"]);

                    Assert.Equal(500, query[2].Counters["Downloads"]);
                    Assert.False(query[2].Counters.ContainsKey("Likes"));
                }
            }
        }

        [Fact]
        public void SessionQuerySelectSingleCounter_UsingRavenQueryCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select RavenQuery.Counter("Downloads");

                    Assert.Equal("from Users select counter(Downloads) as Downloads", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0]);
                    Assert.Equal(200, queryResult[1]);
                    Assert.Null(queryResult[2]);
                }
            }
        }

        [Fact]
        public void SessionQuerySelectSingleCounterWithDocAlias_UsingRavenQueryCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select RavenQuery.Counter(user, "Downloads");

                    Assert.Equal("from Users as user select counter(user, Downloads) as Downloads", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0]);
                    Assert.Equal(200, queryResult[1]);
                    Assert.Null(queryResult[2]);
                }
            }
        }

        [Fact]
        public void SessionQuerySelectCounterAndProjectToNewAnonymousType_UsingRavenQueryCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select new
                                {
                                    DownloadsCount = RavenQuery.Counter(user, "Downloads")
                                };

                    Assert.Equal("from Users as user select counter(user, Downloads) as DownloadsCount", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0].DownloadsCount);
                    Assert.Equal(200, queryResult[1].DownloadsCount);
                    Assert.Null(queryResult[2].DownloadsCount);
                }
            }
        }

        [Fact]
        public void SessionQuerySelectCounterAndProjectToMemberInit_UsingRavenQueryCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select new CounterResult2
                                {
                                    DownloadsCount = RavenQuery.Counter(user, "Downloads")
                                };

                    Assert.Equal("from Users as user select counter(user, Downloads) as DownloadsCount", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0].DownloadsCount);
                    Assert.Equal(200, queryResult[1].DownloadsCount);
                    Assert.Null(queryResult[2].DownloadsCount);
                }
            }
        }

        [Fact]
        public void SessionQuerySelectCounterFromLoadedDoc_UsingRavenQueryCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry", FriendId = "users/2-A" }, "users/1-A");
                    session.Store(new User { Name = "Bob", FriendId = "users/3-A" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen", FriendId = "users/1-A" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from u in session.Query<User>()
                                let f = RavenQuery.Load<User>(u.FriendId)
                                select RavenQuery.Counter(f, "Downloads");

                    Assert.Equal("from Users as u load u.FriendId as f " +
                                 "select counter(f, Downloads) as Downloads", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);

                    Assert.Equal(200, queryResult[0]);
                    Assert.Null(queryResult[1]);
                    Assert.Equal(100, queryResult[2]);

                }
            }
        }

        [Fact]
        public void SessionQuerySelectMultipuleCounters_UsingRavenQueryCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/1-A", "Likes", 200);

                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 400);
                    session.Advanced.Counters.Increment("users/2-A", "Likes", 800);

                    session.Advanced.Counters.Increment("users/3-A", "Likes", 1600);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select new
                                {
                                    Downloads = RavenQuery.Counter(user, "Downloads"),
                                    Likes = RavenQuery.Counter(user, "Likes")
                                };

                    Assert.Equal("from Users as user " +
                                 "select counter(user, Downloads) as Downloads, " +
                                        "counter(user, Likes) as Likes" 
                                , query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0].Downloads);
                    Assert.Equal(200, queryResult[0].Likes);

                    Assert.Equal(400, queryResult[1].Downloads);
                    Assert.Equal(800, queryResult[1].Likes);

                    Assert.Null(queryResult[2].Downloads);
                    Assert.Equal(1600, queryResult[2].Likes);
                }
            }
        }

        [Fact]
        public void SessionQuerySelectSingleCounterWithDocAlias_UsingSessionGetCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select session.Advanced.Counters.Get(user, "Downloads");

                    Assert.Equal("from Users as user select counter(user, Downloads) as Downloads", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0]);
                    Assert.Equal(200, queryResult[1]);
                    Assert.Null(queryResult[2]);
                }
            }
        }

        [Fact]
        public void SessionQuerySelectCounterAndProjectToNewAnonymousType_UsingSessionGetCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select new
                                {
                                    DownloadsCount = session.Advanced.Counters.Get(user, "Downloads")
                                };

                    Assert.Equal("from Users as user select counter(user, Downloads) as DownloadsCount", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0].DownloadsCount);
                    Assert.Equal(200, queryResult[1].DownloadsCount);
                    Assert.Null(queryResult[2].DownloadsCount);
                }
            }
        }

        [Fact]
        public void SessionQuerySelectCounterAndProjectToMemberInit_UsingSessionGetCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select new CounterResult2
                                {
                                    DownloadsCount = session.Advanced.Counters.Get(user, "Downloads")
                                };

                    Assert.Equal("from Users as user select counter(user, Downloads) as DownloadsCount", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0].DownloadsCount);
                    Assert.Equal(200, queryResult[1].DownloadsCount);
                    Assert.Null(queryResult[2].DownloadsCount);
                }
            }
        }

        [Fact]
        public void SessionQuerySelectCounterFromLoadedDoc_UsingSessionGetCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry", FriendId = "users/2-A" }, "users/1-A");
                    session.Store(new User { Name = "Bob", FriendId = "users/3-A" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen", FriendId = "users/1-A" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from u in session.Query<User>()
                                let f = RavenQuery.Load<User>(u.FriendId)
                                select session.Advanced.Counters.Get(f, "Downloads");

                    Assert.Equal("from Users as u load u.FriendId as f " +
                                 "select counter(f, Downloads) as Downloads", query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);

                    Assert.Equal(200, queryResult[0]);
                    Assert.Null(queryResult[1]);
                    Assert.Equal(100, queryResult[2]);

                }
            }
        }

        [Fact]
        public void SessionQuerySelectMultipuleCounters_UsingSessionGetCounter()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry" }, "users/1-A");
                    session.Store(new User { Name = "Bob" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/1-A", "Likes", 200);

                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 400);
                    session.Advanced.Counters.Increment("users/2-A", "Likes", 800);

                    session.Advanced.Counters.Increment("users/3-A", "Likes", 1600);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select new
                                {
                                    Downloads = session.Advanced.Counters.Get(user, "Downloads"),
                                    Likes = session.Advanced.Counters.Get(user, "Likes")
                                };

                    Assert.Equal("from Users as user " +
                                 "select counter(user, Downloads) as Downloads, " +
                                        "counter(user, Likes) as Likes"
                                , query.ToString());

                    var queryResult = query.ToList();

                    Assert.Equal(3, queryResult.Count);
                    Assert.Equal(100, queryResult[0].Downloads);
                    Assert.Equal(200, queryResult[0].Likes);

                    Assert.Equal(400, queryResult[1].Downloads);
                    Assert.Equal(800, queryResult[1].Likes);

                    Assert.Null(queryResult[2].Downloads);
                    Assert.Equal(1600, queryResult[2].Likes);
                }
            }
        }


        [Fact]
        public void SessionQuerySelectSingleCounterJsProjection()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry", Age = 55}, "users/1-A");
                    session.Store(new User { Name = "Bob", Age = 68}, "users/2-A");
                    session.Store(new User { Name = "Pigpen", Age = 27 }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select new
                                {
                                    Name = user.Name + user.Age, //creates js projection
                                    Downloads = RavenQuery.Counter(user, "Downloads")
                                };

                    Assert.Equal("from Users as user select { Name : user.Name+user.Age, Downloads : counter(user, \"Downloads\") }"
                                , query.ToString());

                    var queryResult = query.ToList();
                    Assert.Equal(3, queryResult.Count);

                    Assert.Equal("Jerry55", queryResult[0].Name);
                    Assert.Equal(100, queryResult[0].Downloads);

                    Assert.Equal("Bob68", queryResult[1].Name);
                    Assert.Equal(200, queryResult[1].Downloads);

                    Assert.Equal("Pigpen27", queryResult[2].Name);
                    Assert.Null(queryResult[2].Downloads);

                }
            }
        }

        [Fact]
        public void SessionQuerySelectMultipleCountersJsProjection()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry", Age = 55 }, "users/1-A");
                    session.Store(new User { Name = "Bob", Age = 68 }, "users/2-A");
                    session.Store(new User { Name = "Pigpen", Age = 27 }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                select new
                                {
                                    Likes = session.Advanced.Counters.Get(user, "Likes"),
                                    Downloads = RavenQuery.Counter(user, "Downloads")
                                };

                    Assert.Equal("from Users as user select " +
                                 "counter(user, Likes) as Likes, counter(user, Downloads) as Downloads"
                                , query.ToString());

                    var queryResult = query.ToList();
                    Assert.Equal(3, queryResult.Count);

                    Assert.Null(queryResult[0].Likes);
                    Assert.Equal(100, queryResult[0].Downloads);

                    Assert.Null(queryResult[1].Likes);
                    Assert.Equal(200, queryResult[1].Downloads);

                    Assert.Equal(300, queryResult[2].Likes);
                    Assert.Null(queryResult[2].Downloads);

                }
            }
        }

        [Fact]
        public void SessionQuerySelectCounterViaLet()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry"}, "users/1-A");
                    session.Store(new User { Name = "Bob"}, "users/2-A");
                    session.Store(new User { Name = "Pigpen"}, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                let c = RavenQuery.Counter(user, "Downloads")
                                select new
                                {
                                    Name = user.Name,
                                    Downloads = c
                                };

                    Assert.Equal(
@"declare function output(user) {
	var c = counter(user, ""Downloads"");
	return { Name : user.Name, Downloads : c };
}
from Users as user select output(user)" , query.ToString());

                    var queryResult = query.ToList();
                    Assert.Equal(3, queryResult.Count);

                    Assert.Equal("Jerry", queryResult[0].Name);
                    Assert.Equal(100, queryResult[0].Downloads);

                    Assert.Equal("Bob", queryResult[1].Name);
                    Assert.Equal(200, queryResult[1].Downloads);

                    Assert.Equal("Pigpen", queryResult[2].Name);
                    Assert.Null(queryResult[2].Downloads);

                }
            }
        }

        [Fact]
        public void SessionQuerySelectCounterFromLoadedDocJsProjection()
        {
            using (var store = GetDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    session.Store(new User { Name = "Jerry", FriendId = "users/2-A" }, "users/1-A");
                    session.Store(new User { Name = "Bob", FriendId = "users/3-A" }, "users/2-A");
                    session.Store(new User { Name = "Pigpen", FriendId = "users/1-A" }, "users/3-A");

                    session.Advanced.Counters.Increment("users/1-A", "Downloads", 100);
                    session.Advanced.Counters.Increment("users/2-A", "Downloads", 200);
                    session.Advanced.Counters.Increment("users/3-A", "Likes", 300);

                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = from user in session.Query<User>()
                                let f = RavenQuery.Load<User>(user.FriendId)
                                select new
                                {
                                    Name = user.Name,
                                    Downloads = RavenQuery.Counter(user, "Downloads"),
                                    FriendsDownloads = RavenQuery.Counter(f, "Downloads")
                                };

                    Assert.Equal("from Users as user " +
                                 "load user.FriendId as f " +
                                 "select { Name : user.Name, " +
                                          "Downloads : counter(user, \"Downloads\"), " +
                                          "FriendsDownloads : counter(f, \"Downloads\") }"
                                , query.ToString());

                    var queryResult = query.ToList();
                    Assert.Equal(3, queryResult.Count);

                    Assert.Equal("Jerry", queryResult[0].Name);
                    Assert.Equal(100, queryResult[0].Downloads);
                    Assert.Equal(200, queryResult[0].FriendsDownloads);

                    Assert.Equal("Bob", queryResult[1].Name);
                    Assert.Equal(200, queryResult[1].Downloads);
                    Assert.Null(queryResult[1].FriendsDownloads);

                    Assert.Equal("Pigpen", queryResult[2].Name);
                    Assert.Null(queryResult[2].Downloads);
                    Assert.Equal(100, queryResult[2].FriendsDownloads);

                }
            }
        }

        private class User
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string FriendId { get; set; }
        }

        private class CounterResult
        {
            public long? Downloads { get; set; }
            public long? Likes { get; set; }
            public string Name { get; set; }
        }

        private class CounterResult2
        {
            public long? DownloadsCount { get; set; }
            public long? LikesCount { get; set; }
        }

        private class CounterResult3
        {
            public Dictionary<string, long> Downloads { get; set; }
        }

        private class CounterResult4
        {
            public long? Downloads { get; set; }
            public string Name { get; set; }
            public Dictionary<string, long> Likes { get; set; }
        }

        private class CounterResult5
        {
            public Dictionary<string, long> Counters { get; set; }
        }

        private class CounterResult6
        {
            public long? Counter { get; set; }
        }

        private class CounterResult7
        {
            public long? Downloads { get; set; }
            public long? FriendsDownloads { get; set; }
            public string Name { get; set; }
        }
    }
}