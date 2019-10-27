using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyVet.Web.Data;
using MyVet.Web.Data.Entities;
using MyVet.Web.Helpers;

namespace MyVet.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true; // quiero que el mail sea unico.... ya que es nuestro username es como una clave primary
                // estas son condiciones del password!!!!
                //  ojo estamos en dsrrll
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<DataContext>();



            //declaro el servicio que va a leer la bd del tipo uqe yo quiera
            // es decir si necesito mongdb este es el servicio que voy a cambiar
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Existen 3 formas de inyectar:
            // 1----->adddtransient lo inyecta una sola vez
            services.AddTransient<SeedDb>();
            // recomendacion de buenas practicas que lo inyectes con interface
            // xq te permite q lo cambiens en tiempo de ejecicion y permite pruebas unitarias
            // y uno de los ppios solid es que SW tenga inyeccion de dependencias y que sea testeable
            //2-----> addscope  lo inyecta toda vez q lo llama pero crea una nueva instancia
            //3-----> addsinglenton lo inyecta una vez y lo deja permanente en la ejecucion de vida del py memoria
            //                     que gano con singleton GANO unos milisegundos porque no tengo que einstanciarlo cada vez
            //                     que pierdo PIERDO MEMORIA

            // cq clase q utilice el IuserHelper en el construtor le mando una instancia del USERHELPER
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
