using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PokeApiNet;
using Pokedex.Services;
using System;
using System.Net.Http.Headers;
using Pokedex.Services.Interface;

namespace Pokedex
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Pokedex",
                    Version = "v1",
                    Description = "Pokedex API to fetch Pokemon details & translations"
                });
            });

            services.AddHealthChecks();

            services.AddTransient<IPokeApiNetService,PokeApiNetService>();

            services.AddTransient<IPokemonService,PokemonService>();

            services.AddSingleton<PokeApiClient>();           

            services.AddHttpClient<FunTranslationsService>(c =>
            {
                c.BaseAddress = new Uri("https://api.funtranslations.com/translate/");
                c.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            string redisConnStr = Configuration.GetValue<string>("Redis:ConnectionString");

            if (!string.IsNullOrEmpty(redisConnStr)) // Can use Azure Redis cache instance in production / test environments
            {
                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = redisConnStr;
                    options.InstanceName = "redis-pokedex";
                });
                Console.WriteLine("Redis Cache Initiated.");
            }
            else
            {
                // For Local development environment when not using Redis
                services.AddDistributedMemoryCache();
                Console.WriteLine("Distributed Memory Cache Initiated");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokedex v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/health");

        }
    }
}
