using System;
using System.Timers;
using System.Threading.Tasks;
using FakeUsersAPI.Repositories;
using Microsoft.Extensions.Logging;

namespace FakeUsersAPI.Services
{
    public class Sheduler
    {
        private Timer? timer;
        private CreateFakeUser _userClass;
        private CallDapperDb _db;
        private readonly ILogger<Sheduler> _logger;
        private int count = 0;
        private int posX, posY;

        public Sheduler(CreateFakeUser userClass,
            CallDapperDb db,
            ILogger<Sheduler> logger) 
        {
            _userClass = userClass;
            _db = db;
            _logger = logger;
        }

        public void SetTimer(double interval)
        {
            try
            {
                Console.Clear();
                if (timer != null)
                {
                    timer.Dispose();
                    Console.WriteLine(interval <= 0 ? "Timer is disposed..." : "Old timer is disposed...");
                }
                    Console.WriteLine("Timer with interval {0} sec is started...", interval);
                    posX = Console.CursorLeft; posY = Console.CursorTop;
                    Console.WriteLine("number enters: {0}", count++);
                    timer = new Timer(interval * 1000.0);
                    timer.Elapsed += async (sender, e) => await OnTimedEventAsync();
                    timer.AutoReset = true;
                    timer.Enabled = true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception in Sheduler: {0}", ex);
            }

        }

        private async Task OnTimedEventAsync()
        {
            var usersCount = await _db.GetCountUsersFromDbAsync();
            Console.SetCursorPosition(posX, posY);
            Console.Write("{0}", new string(' ', 20));
            Console.SetCursorPosition(posX, posY);
            if (usersCount < 2) { await _userClass.CreateUserAsync(); }
            else 
            {
                var variantEvent = new Random().Next(1, 3);
                if (variantEvent == 1)
                {
                    await _userClass.CreateUserAsync();
                }
                if (variantEvent == 2)
                {
                    var anyLogin = await _db.GetRandomLoginUserFromDbAsync();
                    await _userClass.CreateUserWithAnyParamsAsync(anyLogin);
                }
            }
            Console.WriteLine("number enters: {0}", count++);
            



        }
    }
}
