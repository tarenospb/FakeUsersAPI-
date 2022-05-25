using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using FakeUsersAPI.Mappers;
using FakeUsersAPI.Services;
using FakeUsersAPI.Repositories;
using Microsoft.Extensions.Hosting;

namespace FakeUsersAPI
{

    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration conf)
        {
            Configuration = conf;
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
           
            appBuilder.UseRouting();
            appBuilder.UseAuthorization();
            appBuilder.UseEndpoints(endpoints => endpoints.MapControllers());
           
            var lifeTime = appBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var appInit = appBuilder.ApplicationServices.GetRequiredService<RabbitClient>();
            
            lifeTime.ApplicationStarted.Register(() =>  appInit.Init());
            lifeTime.ApplicationStopping.Register(() => appInit.Destruct());

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<AppSettingsConnection>(options => Configuration.GetSection("ConnectionStrings").Bind(options));
            services.Configure<AppSettingsChecks>(options => Configuration.GetSection("Checks").Bind(options));
            services.Configure<AppSettingsFreq>(options => Configuration.GetSection("CheckEnterFreq").Bind(options));
            services.AddSingleton<CreateFakeUser>();
            services.AddSingleton<IpLocateMapper>();
            services.AddSingleton<CallDapperDb>();
            services.AddSingleton<RabbitClient>();
            services.AddSingleton<UserDataMapper>();
            services.AddSingleton<EnterDataMapper>();
            services.AddSingleton<CoordLocate>();
            services.AddSingleton<ValidateUserAndWriteResult>();
            services.AddSingleton<SqlCommandsForDb>();
            services.AddSingleton<PassValidate>();
            services.AddSingleton<EnterValidate>();
            services.AddSingleton<Sheduler>();


        }
    }
}
