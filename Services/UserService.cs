using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using Authy.Models;
using Microsoft.AspNetCore.Http;

namespace Authy.Services {
  public class UserService {
    private DirectorySearcher ds { get; }

    public UserService() {
      ds = new DirectorySearcher();
    }

    private User Serialize(SearchResult x) {
      return new User {
        AccountName = x.Properties["sAMAccountName"][0] as string,
        DisplayName = x.Properties["displayname"][0] as string
      };
    }

    private string DirectoryQuery(string predicates) {
      var basePredicates = "(objectCategory=user)";
      return $"(&{basePredicates}{predicates})";
    }

    public User GetByAccountName(string accountName) {
      var p = $"(sAMAccountName={accountName})";
      ds.Filter = DirectoryQuery(p);
      SearchResult x = ds.FindOne();
      return Serialize(x);
    }

    public User WhoAmI(HttpContext ctx) {
      var userName = ctx.User.Identity.Name.Split('\\')[1];
      var u = GetByAccountName(userName);
      return u;
    }

    public List<User> GetByAccountNames(List<string> names) {
      var qs = names.Aggregate("", (acc, x) => $"{acc}(sAMAccountName={x})");
      var p = $"(|{qs})";
      ds.Filter = DirectoryQuery(p);
      var xs = ds.FindAll();

      var users = new List<User>();
      foreach (SearchResult x in xs) {
        var u = Serialize(x);
        users.Add(u);
      }

      // prevent memory leaks
      // see https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.directorysearcher.findall?view=netframework-4.7.2
      xs.Dispose();

      return users;
    }
  }
}