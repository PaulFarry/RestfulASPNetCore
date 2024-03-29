﻿using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Entities;
using RestfulASPNetCore.Web.Helpers;
using RestfulASPNetCore.Web.Services;
using System.Linq;


namespace RestfulASPNetCore.Web
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
            services.AddMvc(
                setup =>
                {
                    setup.ReturnHttpNotAcceptable = true;
                    setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                    var xmlInputFormatter = new XmlDataContractSerializerInputFormatter(new MvcOptions { });
                    xmlInputFormatter.SupportedMediaTypes.Add(VendorMediaType.NewAuthorDeadXml);

                    setup.InputFormatters.Add(xmlInputFormatter);

                    var jsonOutputFormatter = setup.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
                    if (jsonOutputFormatter != null)
                    {
                        jsonOutputFormatter.SupportedMediaTypes.Add(VendorMediaType.HateoasLinks);
                    }


                    var jsonInputFormatter = setup.InputFormatters.OfType<NewtonsoftJsonInputFormatter>().FirstOrDefault();
                    if (jsonInputFormatter != null)
                    {
                        jsonInputFormatter.SupportedMediaTypes.Add(VendorMediaType.NewAuthor);
                        jsonInputFormatter.SupportedMediaTypes.Add(VendorMediaType.NewAuthorDead);
                    }
                    //setup.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
                }
            ).AddNewtonsoftJson(options =>
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
            )
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllersWithViews(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            });
            var connectionString = Configuration["connectionStrings:libraryDBConnectionString"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<ILibraryRepository, LibraryRepository>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(imp =>
               {
                   var actionContext = imp.GetService<IActionContextAccessor>().ActionContext;
                   return new UrlHelper(actionContext);
               }
            );

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddHttpCacheHeaders(
            options =>
                {
                    options.MaxAge = 600;
                },
            validationOptions =>
                {
                    validationOptions.AddMustRevalidate = true;
                }
            );

            services.AddResponseCaching();

            services.AddMemoryCache();

            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddMemoryCache();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.Configure<IpRateLimitOptions>((options) =>
            {
                options.GeneralRules = new System.Collections.Generic.List<RateLimitRule>()
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Limit = 1000,
                        Period = "5m"
                        //PeriodTimespan = new TimeSpan(0,5,0)
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Limit = 200,
                        Period = "10s"
                        //PeriodTimespan = new TimeSpan(0,0,10)
                    }
                };

            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();



        }


        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LibraryContext libraryContext)
        {

            //.Net core 1.1 approach
            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            //loggerFactory.AddNLog();


            if (env.EnvironmentName.Equals("development", System.StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Author, Dtos.Author>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                        src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

                cfg.CreateMap<Entities.Book, Dtos.Book>();

                cfg.CreateMap<CreateAuthor, Entities.Author>();
                cfg.CreateMap<CreateDeadAuthor, Entities.Author>();

                cfg.CreateMap<CreateBook, Entities.Book>();
                cfg.CreateMap<UpdateBook, Entities.Book>();
                cfg.CreateMap<Entities.Book, UpdateBook>();

            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            libraryContext.EnsureSeedDataForContext();

            app.UseIpRateLimiting();

            app.UseResponseCaching();

            app.UseHttpCacheHeaders();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
        }
    }
}
