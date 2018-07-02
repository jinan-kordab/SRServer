using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Transports;

[assembly: OwinStartup(typeof(SignalRSelfHost.Startup))]
namespace SignalRSelfHost
{
	class Program
	{
		static void Main(string[] args)
        { 
			string url = "http://localhost:8080";
			using (WebApp.Start(url))
			{
				Console.WriteLine("Server running on {0}", url);
				Console.ReadLine();
			}
		}
	}

	class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseCors(CorsOptions.AllowAll);
			app.MapSignalR();
		}
	}

	public class MyHub : Hub
	{
		// database
		SRServer.EntityFramework.MySignalRUsersEntities db = new SRServer.EntityFramework.MySignalRUsersEntities();

		public override Task OnConnected()
		{
			//add to the database
			string ClientconnecetionID = Context.ConnectionId;
			Console.WriteLine("ConnectionId: " + ClientconnecetionID);

			using (var db = new SRServer.EntityFramework.MySignalRUsersEntities())
			{
				SRServer.EntityFramework.UsersTemp usersTemp = new SRServer.EntityFramework.UsersTemp
				{
					UniqueUserID = ClientconnecetionID
				};

				db.UsersTemp.Add(usersTemp);
				db.SaveChanges();
			}
			
			return base.OnConnected();
		}


		public override Task OnDisconnected(bool stopCalled)
		{
			string ClientconnecetionID = Context.ConnectionId;

			using (var db = new SRServer.EntityFramework.MySignalRUsersEntities())
			{
				var rToDel = db.UsersTemp.SingleOrDefault(x => x.UniqueUserID == ClientconnecetionID);

				if (rToDel != null)
				{
					db.UsersTemp.Remove(rToDel);
					db.SaveChanges();
				}
			}

			return base.OnDisconnected(stopCalled);
		}

		public void Send(string name, string message)
		{
			string ClientconnecetionID = this.Context.ConnectionId;
			LoggedInUsers();
			Clients.Client(name).addMessage(name, message);
		}

		public void LoggedInUsers()
		{
			string ClientconnecetionID = Context.ConnectionId;
			Console.WriteLine(ClientconnecetionID);
		}

	}
}