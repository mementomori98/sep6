using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Authentication;
using Core.Domain.DiscussionItems;
using Core.Domain.Movies;
using Core.Domain.Recommendations;
using Core.Domain.Toplists;
using Core.Domain.Toplists.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Notflix.Utils;

namespace Notflix
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMudServices(c =>
            {
                c.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                c.SnackbarConfiguration.NewestOnTop = true;
                c.SnackbarConfiguration.VisibleStateDuration = 2000;
                c.SnackbarConfiguration.ShowTransitionDuration = 500;
                c.SnackbarConfiguration.HideTransitionDuration = 500;
                c.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });
            services.AddTransient<IMovieService, OmdbMovieService>();
            services.AddTransient<IRecommendationService, RecommendationsService>();
            services.AddTransient<IDiscussionItemService, DiscussionItemService>();
            services.AddTransient<LocalStorage>();
            services.AddTransient<IMovieService, OmdbMovieService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IToplistService, ToplistService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
