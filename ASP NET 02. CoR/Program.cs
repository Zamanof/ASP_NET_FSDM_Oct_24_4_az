using ASP_NET_02._CoR;
using ASP_NET_02._CoR.Concrete;

User user = new() { UserName = "salam", Password = "Salam12345", Email = "salam@salam.com" };
var director = new CheckDirector();
Console.WriteLine(director.MakeUserChecker(user));
