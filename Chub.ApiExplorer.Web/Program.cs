namespace Chub.ApiExplorer.Web
{
    using Chub.ApiExplorer.Web.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Stylelabs.M.Sdk.WebClient;

    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient();

            builder.Services.AddTransient(serviceProvider =>
            {
                IWebMClient client = MClientFactory.CreateMClient(builder.Configuration.GetConnectionString("ContentHub"));

                return client;
            });

            builder.Services.AddTransient<ITabSearchModelBuilder, UserPageService>();
            builder.Services.AddTransient<IUserPageService, UserPageService>();

            builder.Services.AddTransient<ITabSearchModelBuilder, AssetPageService>();
            builder.Services.AddTransient<IAssetPageService, AssetPageService>();

            builder.Services.AddTransient<IEntityDefinitionPageService, EntityDefinitionPageService>();
            builder.Services.AddTransient<ITabSearchModelBuilder, EntityDefinitionPageService>();

            builder.Services.AddTransient<ITabSearchModelBuilder, DatasourcePageService>();
            builder.Services.AddTransient<IDatasourcePageService, DatasourcePageService>();

            builder.Services.AddTransient<ITabSearchModelBuilder, PagePageService>();
            builder.Services.AddTransient<IPagePageService, PagePageService>();

            IMvcBuilder mvcBuilder = builder.Services.AddControllersWithViews();

#if DEBUG
            mvcBuilder.AddRazorRuntimeCompilation();
#endif

            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}