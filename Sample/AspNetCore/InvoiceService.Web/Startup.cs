using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using DioDocs.FastReportBuilder;
using GrapeCity.Documents.Excel;
using InvoiceService.Repository;
using InvoiceService.Repository.SqlServer;
using InvoiceService.Transaction;
using InvoiceService.UseCase;
using InvoiceService.UseCase.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Extras.DynamicProxy;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace InvoiceService.Web
{
    public class Startup : IConnectionFactory, ITemplateProvider
    {
        private readonly Container _container = new Container();

        private readonly string _connectionString;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _connectionString = Configuration.GetConnectionString("DioDocs");

            Workbook.SetLicenseKey(Configuration.GetValue<string>("DioDocsForExcelKey"));
            C1.Web.Mvc.LicenseManager.Key = Configuration.GetValue<string>("C1Key");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            IntegrateSimpleInjector(services);
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(
                new SimpleInjectorViewComponentActivator(_container));

            services.EnableSimpleInjectorCrossWiring(_container);
            services.UseSimpleInjectorAspNetRequestScoping(_container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            InitializeContainer(app);
            _container.Verify();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            // Add application services. For instance:
            _container.Register<IBuildInvoice, BuildInvoice>(Lifestyle.Scoped);
            _container.Register<ISalesOrderRepository, SalesOrderRepository>(Lifestyle.Scoped);
            _container.Register<IInvoiceRepository, InvoiceRepository>(Lifestyle.Scoped);
            _container.Register<IReportBuilderFactory, ReportBuilderFactory>(Lifestyle.Scoped);
            _container.Register<ITemplateProvider>(() => this, Lifestyle.Scoped);

            _container.Register<ITransactionContext, TransactionContext>(Lifestyle.Scoped);
            _container.InterceptWith<TransactionInterceptor>(
                x => x.Namespace == typeof(ISalesOrderRepository).Namespace);
            _container.Register<IConnectionFactory>(() => this);

            // Allow Simple Injector to resolve services from ASP.NET Core.
            _container.AutoCrossWireAspNetComponents(app);
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }

        private readonly HttpClient _httpClient = new HttpClient();

        public byte[] Get()
        {
            var task = _httpClient.GetByteArrayAsync(Configuration.GetValue<string>("TemplateUrl"));
            task.Wait();
            return task.Result;
        }
    }
}
